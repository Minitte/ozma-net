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

namespace wpf_ozma_net
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private Stack<StrokeAction> m_undoStack;
        private Stack<StrokeAction> m_redoStack;

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
            WriteableBitmap modImg = new WriteableBitmap(img);

            ScaleTransform s = new ScaleTransform(0.1, 0.1);

            TransformedBitmap res = new TransformedBitmap(img, s);

            NetworkImage.Source = res;
        }

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
    }
}
