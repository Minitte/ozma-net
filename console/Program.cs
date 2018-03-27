using System;

using ozmanet.neural_network;
using ozmanet.util;

namespace console
{
    class Program
    {
        static void Main(string[] args)
        {
            int[] layerSettings = { 784, 15, 10 };
            Network network = new Network(layerSettings);

            MnistReader reader = new MnistReader(
            "../../../../data/digits/training/train-labels.idx1-ubyte",     // path for labels
            "../../../../data/digits/training/train-images.idx3-ubyte");    // path for images

            float[,] inputSets = new float[50, 784];
            float[,] expectedSets = new float[50, 10];

            int setIndex = 0;
            int numSets = 0;

            while (reader.HasNext() && numSets < 300)
            {
                CharacterImage input = reader.ReadNext();
                byte[] dataBytes = input.DataTo1D();

                for (int i = 0; i < dataBytes.Length; i++)
                {
                    inputSets[setIndex, i] = (float)dataBytes[i] / 255f;
                }

                int expected = input.Value - '0';

                expectedSets[setIndex, expected] = 1f;

                setIndex++;

                if (setIndex == 50)
                {
                    network.Learn(inputSets, expectedSets);
                    numSets++;
                    setIndex = 0;
                }
            }

            reader.Dispose();

            reader = new MnistReader(
                "../../../../data/digits/training/train-labels.idx1-ubyte",     // path for labels
                "../../../../data/digits/training/train-images.idx3-ubyte");    // path for images

            while (reader.HasNext())
            {
                CharacterImage input = reader.ReadNext();
                byte[] datab = input.DataTo1D();
                float[] dataf = new float[datab.Length];
                if (dataf.Length != 28 * 28)
                {
                    Console.WriteLine("");
                }

                for (int i = 0; i < datab.Length; i++)
                {
                    dataf[i] = (float)datab[i] / 255f;
                }

                int expected = input.Value - '0';
                float[] expectedArray = new float[10];

                expectedArray[expected] = 1;

                int actual = network.FeedForward(dataf);

                Console.WriteLine("Expected: " + expected + " Actual: " + actual);
            }


            Console.Write("Press any key to end");
            Console.ReadKey();
        }
    }
}
