using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.IO;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace mnist_data_creator
{
    class MnistDataCreator
    {
        static void Main(string[] args)
        {
            bool running = true;
            
            while (running)
            {
                WriteMenu();
                string input = Console.ReadLine();

                int choice = -1;

                if (input.Equals("dir"))
                {
                    string[] dirs = Directory.GetDirectories(Directory.GetCurrentDirectory());
                    foreach (string d in dirs)
                    {
                        Console.WriteLine(d);
                    }
                    continue;
                }

                bool correct = int.TryParse(input, out choice);

                if (!correct)
                {
                    Console.WriteLine("Invalid choice!");
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        // input path
                        Console.WriteLine("From " + Directory.GetCurrentDirectory() + "...");
                        Console.WriteLine("Enter input folder path.");
                        string dirPath = Console.ReadLine();

                        if (!Directory.Exists(dirPath))
                        {
                            Console.WriteLine("Can't find the folder!");
                            Console.WriteLine(Directory.GetCurrentDirectory() + "\\" + dirPath);
                            break;
                        }

                        // width, height, max and amt
                        int width = 0;
                        int height = 0;
                        int max = 0;
                        int amt = 0;

                        Console.WriteLine("\nImage will be resize to fit the width and height..");
                        Console.WriteLine("\nWidth? (0 to skip resizing)");
                        if (!int.TryParse(Console.ReadLine(), out width)) {
                            Console.WriteLine("Could not read that number!");
                            break;
                        }

                        if (width != 0)
                        {
                            Console.WriteLine("\nHeight?");
                            if (!int.TryParse(Console.ReadLine(), out height))
                            {
                                Console.WriteLine("Could not read that number!");
                                break;
                            }
                        }

                        Console.WriteLine("\nMax images per folder? (0 for unlimited)");
                        if (!int.TryParse(Console.ReadLine(), out max))
                        {
                            Console.WriteLine("Could not read that number!");
                            break;
                        }

                        Console.WriteLine("\nMax number of images total? (0 for unlimited)");
                        if (!int.TryParse(Console.ReadLine(), out amt))
                        {
                            Console.WriteLine("Could not read that number!");
                            break;
                        }

                        // output path
                        Console.WriteLine("\nEnter output path");
                        string outPath = Console.ReadLine();

                        FolderToMnist(dirPath, outPath, width, height, max, amt);
                        break;

                    case 6:
                        Console.WriteLine("Ending...");
                        running = false;
                        break;

                    default:
                        Console.WriteLine("Invalid choice!");
                        break;
                }
            }

            Console.WriteLine("Press any key to end...");
            Console.ReadKey();
        }

        /// <summary>
        /// outputs menu
        /// </summary>
        static void WriteMenu()
        {
            Console.WriteLine("===============");
            Console.WriteLine("1 - from Folder");
            Console.WriteLine("6 - exit");
            Console.WriteLine("dir - list dirs");
            Console.WriteLine("===============");
        }

        /// <summary>
        /// reads all images from folder
        /// </summary>
        static void FolderToMnist(string dirPath, string outPath, int width, int height, int max, int amt)
        {
            // list of all the folders
            string[] folders = Directory.GetDirectories(dirPath);

            List<GeneralImage> imgList = new List<GeneralImage>();
            List<string> labelList = new List<string>();

            int countTotal = 0;
            byte labelIndex = 0;

            if (max <= 0)
            {
                max = int.MaxValue - 1;
            }

            if (amt <= 0)
            {
                amt = int.MaxValue - 1;
            }

            Console.WriteLine("\nSearching for images...");

            foreach (string f in folders)
            {
                // list of all of the imgs in the folder
                string[] imgPath = Directory.GetFiles(f);

                string fullPath = Path.GetFullPath(f).TrimEnd(Path.DirectorySeparatorChar);
                string label = Path.GetFileName(fullPath);
                labelList.Add(label);
                int count = 0;
                int resizeCount = 0;

                // load all of the images in the folder
                foreach (string i in imgPath)
                {
                    // limit per folder
                    if (++count > max)
                    {
                        break;
                    }
                    countTotal++;
                    Console.Write("\rFound " + count + " in " + label + "(" + labelIndex + "), resized " + resizeCount + " images");

                    // load and hold
                    Image img = Image.FromFile(i);

                    if ((width != 0 && height != 0) && (width != img.Width || height != img.Height))
                    {
                        img = ResizeImage(img, width, height);
                        resizeCount++;
                    }

                    Bitmap bmp = new Bitmap(img);
                    byte[,] data = BitmapToByteArr(bmp);
                    //writer.WriteImage(data);
                    imgList.Add(new GeneralImage(data, labelIndex));
                }
                labelIndex++;
                Console.WriteLine();
            }

            Console.WriteLine("\nFound a total of " + countTotal + " images");

            Console.WriteLine("\nShuffling list...");
            Shuffle(imgList);

            Console.WriteLine("\nWriting images to file...");

            // begin writing data
            int numData = amt < int.MaxValue - 2 ? amt : imgList.Count;

            if (width == 0)
            {
                width = imgList[0].Image.GetLength(0);
                height = imgList[0].Image.GetLength(1);
            }

            MnistImageWriter imgWriter = new MnistImageWriter(outPath, (uint)numData, (uint)width, (uint)height);
            MnistLabelWriter labelWriter = new MnistLabelWriter(outPath + "-labels", (uint)numData);

            int writeCount = 0;

            foreach (GeneralImage gimg in imgList)
            {
                if (++writeCount > amt)
                {
                    break;
                }
                imgWriter.WriteImage(gimg.Image);
                labelWriter.WriteLabel(gimg.LabelIndex);
                Console.Write("\r wrote " + writeCount + " out of " + countTotal);
            }
            Console.WriteLine();

            if (amt > 0 && amt < int.MaxValue - 2)
            {
                Console.WriteLine((countTotal - amt) + " images was skipped due to user defined limit!");
            }

            imgWriter.Close();
            labelWriter.Close();

            // write label strings for reference
            StreamWriter txtWriter = new StreamWriter(outPath + "-labels-string.txt");
            foreach (string l in labelList)
            {
                txtWriter.WriteLine(l);
            }
            txtWriter.Close();

            Console.WriteLine("Done");
        }

        static byte[,] BitmapToByteArr(Bitmap img)
        {
            byte[,] data = new byte[img.Width, img.Height];

            for (int x = 0; x < img.Width; x++)
            {
                for (int y = 0; y < img.Height; y++)
                {
                    // pixel
                    Color px = img.GetPixel(x, y);
                    int grey = (px.R + px.G + px.B) / 3;

                    data[x, y] = (byte)(grey > 255 ? 255 : grey);
                }
            }

            return data;
        }

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        public static void Shuffle(List<GeneralImage> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                GeneralImage value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
