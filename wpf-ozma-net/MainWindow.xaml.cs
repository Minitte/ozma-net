using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Ink;

using ozmanet.neural_network;
using ozmanet.util;
using Microsoft.Win32;

namespace wpf_ozma_net
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private Stack<StrokeAction> m_undoStack;
        private Stack<StrokeAction> m_redoStack;
        private Network m_network;

        public MainWindow()
        {
            InitializeComponent();

            m_undoStack = new Stack<StrokeAction>();
            m_redoStack = new Stack<StrokeAction>();
        }

        /// <summary>
        /// Converts the drawing from the app into a byte array
        /// </summary>
        /// <returns></returns>
        private byte[] DrawingToBitmap()
        {
            //get the dimensions of the ink control
            int margin = (int)this.DrawingCanvas.Margin.Left;
            int width = (int)this.DrawingCanvas.ActualWidth - margin;
            int height = (int)this.DrawingCanvas.ActualHeight - margin;
            //render ink to bitmap
            RenderTargetBitmap rtb =
            new RenderTargetBitmap(width, height, 96d, 96d, PixelFormats.Default);
            rtb.Render(DrawingCanvas);
            //save the ink to a memory stream
            BmpBitmapEncoder encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rtb));
            byte[] bitmapBytes;
            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                //get the bitmap bytes from the memory stream
                ms.Position = 0;
                bitmapBytes = ms.ToArray();
            }
            return bitmapBytes;
        }

        /// <summary>
        /// Converts byte array into bitmapimage object
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public BitmapImage ToImage(byte[] array)
        {
            using (var ms = new System.IO.MemoryStream(array))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad; // here
                image.StreamSource = ms;
                image.EndInit();
                return image;
            }
        }

        /// <summary>
        /// Updates the network image view
        /// </summary>
        private void UpdateNetworkImage()
        {

            byte[] data = DrawingToBitmap();

            BitmapImage img = ToImage(data);

            ScaleTransform s = new ScaleTransform(0.1, 0.1);

            TransformedBitmap res = new TransformedBitmap(img, s);
            
            NetworkImage.Source = res;

            // ask network
            if (m_network == null)
            {
                return;
            }

            float[] input = ConvertToNetworkInput(res);

            int result = m_network.FeedForward(input);

            if (result == -2)
            {
                NetworkResultText.Text = "???";
            }
            else
            {
                NetworkResultText.Text = result + "!";
            }
        }

        /// <summary>
        /// Converts the bitmapimage into a 2d byte array that is more suitable for the network's input
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        private float[] ConvertToNetworkInput(TransformedBitmap img)
        {
            int w = (int)(img.Width + 0.5);
            int h = (int)(img.Height + 0.5);
            float[] data = new float[w * h];
            int[] pxBuffer = new int[w * h];

            int stride = ((img.PixelWidth * img.Format.BitsPerPixel) + 7) / 8;

            // copy argb pixels to buffer
            img.CopyPixels(pxBuffer, stride, 0);

            // for all pixels in buffer
            for (int i = 0; i < h * w; i++)
            {
                
                // extract rgb values
                int px = pxBuffer[i];
                int r = px & 0x00ff0000;
                r = r >> 16;
                r = r ^ 255;
                int g = px & 0x0000ff00;
                g = g >> 8;
                g = g ^ 255;
                int b = px & 0x000000ff;
                b = b ^ 255;

                // calculate y value
                //data[i] = ((0.299f * r) + (0.567f * g) + (0.114f * b)) / 255f;

                // alt, gray average
                data[i] = ((r + g + b) / 3f) / 255f;

                // darkening
                if (data[i] != 0)
                {
                    data[i] *= 1.5f;
                    data[i] = data[i] > 1.0f ? 1.0f : data[i];
                }
            }
            Console.WriteLine(ToStringArt(data));
            return data;
        }

        public string ToStringArt(float[] pixels)
        {
            string s = "";
            for (int i = 0; i < 28; ++i)
            {
                for (int j = 0; j < 28; ++j)
                {
                    if (pixels[(i * 28) + j] == 0)
                        s += " "; // white
                    else if (pixels[(i * 28) + j] > 0.9)
                        s += "O"; // black
                    else
                        s += "."; // gray
                }
                s += "\n";
            }
            return s;
        } // ToString

        /// <summary>
        /// Undo last action
        /// </summary>
        private void UndoStroke()
        {
            if (m_undoStack.Count() != 0)
            {
                StrokeAction action = m_undoStack.Pop();

                if (action.Add)
                {
                    foreach (Stroke s in action.StrokeList)
                    {
                        DrawingCanvas.Strokes.Remove(s);
                    }
                }
                else {
                    foreach (Stroke s in action.StrokeList)
                    {
                        DrawingCanvas.Strokes.Add(s);
                    }
                }

                action.Add = !action.Add;
                m_redoStack.Push(action);
            }
        }

        /// <summary>
        /// Redo undone actions
        /// </summary>
        private void RedoStroke()
        {
            if (m_redoStack.Count() != 0)
            {
                StrokeAction action = m_redoStack.Pop();

                if (action.Add)
                {
                    foreach (Stroke s in action.StrokeList)
                    {
                        DrawingCanvas.Strokes.Remove(s);
                    }
                }
                else
                {
                    foreach (Stroke s in action.StrokeList)
                    {
                        DrawingCanvas.Strokes.Add(s);
                    }
                }

                action.Add = !action.Add;
                m_undoStack.Push(action);
            }
        }

        /// <summary>
        /// Class that holds information for proper undo/redo 
        /// </summary>
        public class StrokeAction
        {
            public List<Stroke> StrokeList;
            public bool Add;

            public StrokeAction(List<Stroke> list, bool add)
            {
                this.Add = add;
                this.StrokeList = list;
            }
        }

        /// <summary>
        /// Undo button event that triggers a undo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnUndoEvent(object sender, RoutedEventArgs e)
        {
            UndoStroke();
        }

        /// <summary>
        /// redo button event that triggers a redo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnRedoEvent(object sender, RoutedEventArgs e)
        {
            RedoStroke();
        }

        /// <summary>
        /// Event handler for when the user draws
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StrokeDrawnEvent(object sender, InkCanvasStrokeCollectedEventArgs e)
        {
            List<Stroke> strokeList = new List<Stroke>();
            strokeList.Add(e.Stroke);
            m_undoStack.Push(new StrokeAction(strokeList, true));

            byte[] data = DrawingToBitmap();

            m_redoStack.Clear();

            UpdateNetworkImage();
        }

        /// <summary>
        /// Button click event for clearing the drawing canvas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnClearEvent(object sender, RoutedEventArgs e)
        {
            List<Stroke> strokeList = new List<Stroke>();

            foreach (Stroke s in DrawingCanvas.Strokes)
            {
                strokeList.Add(s);
            }

            m_undoStack.Push(new StrokeAction(strokeList, false));

            DrawingCanvas.Strokes.Clear();
        }

        private void BtnLoadNetworkEvent(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Ozma Network (*.ozmanet)|*.ozmanet";
            dialog.FilterIndex = 1;
            dialog.RestoreDirectory = true;

            if (dialog.ShowDialog() == true)
            {
                LoadNetwork(dialog.FileName);
            }
        }

        private void LoadNetwork(String path)
        {
            // init network
            //FileStream stream = new FileStream(path, FileMode.Open);
            //StreamReader reader = new StreamReader(stream);

            NetworkLoader loader = new NetworkLoader(path);
            m_network = loader.Load();

            //reader.Close();
            //stream.Close();
            loader.Dispose();
        }

        private void SaveBitmapAsPNG(BitmapSource img, String filePath)
        {
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(img));
                encoder.Save(fileStream);
            }
        }

        private void BtnSaveNetworkImgEvent(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "PNG (*.png)|*.png";
            dialog.FilterIndex = 1;
            dialog.RestoreDirectory = true;

            if (dialog.ShowDialog() == true)
            {

                byte[] data = DrawingToBitmap();

                BitmapImage img = ToImage(data);
                WriteableBitmap modImg = new WriteableBitmap(img);

                ScaleTransform s = new ScaleTransform(0.1, 0.1);

                TransformedBitmap res = new TransformedBitmap(img, s);

                SaveBitmapAsPNG(res, dialog.FileName);
            }
        }
    }
}
