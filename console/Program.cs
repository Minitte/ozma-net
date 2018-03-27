using System;

using ozmanet.neural_network;
using ozmanet.util;

namespace console
{
    class Program
    {
        static void Main(string[] args)
        {
            //int[] layerSettings = { 1, 4, 1 };
            //Network network = new Network(layerSettings);

            MnistReader reader = new MnistReader(
                    "../../../../data/digits/test/t10k-labels.idx1-ubyte",     // path for labels
                    "../../../../data/digits/test/t10k-images.idx3-ubyte");    // path for imgs

            while (reader.HasNext())
            {
                CharacterImage img = reader.ReadNext();

                Console.WriteLine(ToString(img));

                Console.ReadKey();
            }

            Console.Write("Press any key to end");
            Console.ReadKey();
        }

        public static string ToString(CharacterImage img)
        {
            byte[,] pixels = img.Data;
            string s = "";
            for (int y = 0; y < 28; y++)
            {
                for (int x = 0; x < 28; x++)
                {
                    if (pixels[x, y] == 0)
                        s += " "; // white
                    else if (pixels[x, y] == 255)
                        s += "O"; // black
                    else
                        s += "."; // gray
                }
                s += "\n";
            }
            s += img.Value;
            return s;
        } // ToString

    }
}
