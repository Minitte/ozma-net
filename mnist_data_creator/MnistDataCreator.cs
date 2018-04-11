using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.IO;
using ozmanet.util;
using ozmanet.neural_network;

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

                        // width and height
                        int width = 0;
                        int height = 0;

                        Console.WriteLine("Width?");
                        if (!int.TryParse(Console.ReadLine(), out width)) {
                            Console.WriteLine("Could not read that number!");
                            break;
                        }

                        Console.WriteLine("Height?");
                        if (!int.TryParse(Console.ReadLine(), out height))
                        {
                            Console.WriteLine("Could not read that number!");
                            break;
                        }

                        // output path
                        Console.WriteLine("Enter output path");
                        string outPath = Console.ReadLine();

                        FolderToMnist(dirPath, outPath, (uint)width, (uint)height);
                        WriteMenu();
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
        static void FolderToMnist(string dirPath, string outPath, uint width, uint height)
        {
            // list of all the folders
            string[] folders = Directory.GetDirectories(dirPath);

            List<GeneralImage> imgList = new List<GeneralImage>();

            int countTotal = 0;
            byte labelIndex = 0; ;

            foreach (string f in folders)
            {
                // list of all of the imgs in the folder
                string[] imgPath = Directory.GetFiles(f);

                string fullPath = Path.GetFullPath(dirPath).TrimEnd(Path.DirectorySeparatorChar);
                string label = Path.GetFileName(fullPath);
                int count = 0;

                // load all of the images in the folder
                foreach (string i in imgPath)
                {
                    Console.Write("\r Found " + ++count + " in " + label + "(" + labelIndex + ")");
                    Image img = Image.FromFile(i);
                    Bitmap bmp = new Bitmap(img);
                    byte[,] data = BitmapToByteArr(bmp);
                    //writer.WriteImage(data);
                    imgList.Add(new GeneralImage(data, labelIndex));
                }
                labelIndex++;
                Console.WriteLine();
            }

            Console.WriteLine("Found a total of " + countTotal + "images");

            // begin writing data
            MnistImageWriter imgWriter = new MnistImageWriter(outPath, (uint)imgList.Count, width, height);
            MnistLabelWriter labelWriter = new MnistLabelWriter(outPath + "-labels", (uint)imgList.Count);

            int writeCount = 0;

            foreach (GeneralImage gimg in imgList)
            {
                imgWriter.WriteImage(gimg.Image);
                labelWriter.WriteLabel(gimg.LabelIndex);
                Console.Write("\r wrote " + ++writeCount + "out of " + countTotal);
            }

            imgWriter.Close();
            labelWriter.Close();

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
    }
}
