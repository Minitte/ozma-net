using System;

using ozmanet.neural_network;
using ozmanet.util;

namespace console
{
    class Program
    {

        static int alterateFile = 1;

        static void Main(string[] args)
        {
            int[] layersettings = { 784, 30, 10 };
            //Network network = new Network(layersettings);

            String savePath = "1digit-net.ozmanet";
            NetworkLoader loader = new NetworkLoader(savePath);
            Network network = loader.Load();
            loader.Dispose();

            int numSets = 10;
            int learningIterations = 60000;

            for (int run = 0; run < 1; run++)
            {
                Console.WriteLine("----- Run #" + run + "-----");

                //TrainNetwork(network, learningIterations, numSets);

                TestNetwork(network);

                //SaveNetwork(network, savePath);
            }

            Console.WriteLine("Done");
            Console.ReadKey();
        }

        static void TrainNetwork(Network network, int learningIterations, int numSets)
        {
            
            network.TotalHits = 0;

            MnistReader reader = new MnistReader(
                "../data/digits/training/train-labels.idx1-ubyte",     // path for labels
                "../data/digits/training/train-images.idx3-ubyte");    // path for imgs

            int iterations = 0;

            while (reader.HasNext() && iterations < learningIterations)
            {
                iterations++;
                float[,] inputSet = new float[numSets, 784];
                float[,] expectedSet = new float[numSets, 10];

                for (int i = 0; i < numSets; i++)
                {
                    CharacterImage input = reader.ReadNext();
                    byte[] dataB = DataTo1D(input.Data);
                    float[] dataF = new float[dataB.Length];

                    for (int j = 0; j < dataB.Length; j++)
                    {
                        dataF[j] = ((float)dataB[j]) / 255f;
                    }

                    float[] expected = new float[10];
                    int value = input.Value - '0';
                    expected[value] = 1f;

                    Buffer.BlockCopy(dataF, 0, inputSet, i * 4 * inputSet.GetLength(1), 4 * dataF.Length);
                    Buffer.BlockCopy(expected, 0, expectedSet, i * 4 * expectedSet.GetLength(1), 4 * expected.Length);
                }

                network.Learn(inputSet, expectedSet);

                Console.Write("\r Learning Hits: " + network.TotalHits + " / " + iterations * numSets + " (" + (int)(((float)network.TotalHits / ((float)iterations * (float)numSets)) * 100) + "%)");

            }

            reader.Dispose();
        }

        static void TestNetwork(Network network)
        {
            MnistReader reader = new MnistReader(
                    "../data/digits/test/t10k-labels.idx1-ubyte",     // path for labels
                    "../data/digits/test/t10k-images.idx3-ubyte");    // path for imgs

            int hits = 0;
            int iterations = 0;
            Console.WriteLine();

            while (reader.HasNext())
            {
                iterations++;

                CharacterImage input = reader.ReadNext();
                byte[] dataB = DataTo1D(input.Data);
                float[] dataF = new float[dataB.Length];

                for (int j = 0; j < dataB.Length; j++)
                {
                    dataF[j] = ((float)dataB[j]) / 255f;
                }

                float[] expected = new float[10];
                int value = input.Value - '0';
                expected[value] = 1f;

                if (network.FeedForward(dataF) == value)
                {
                    hits++;
                }

                Console.Write("\r Hits: " + hits + " / " + iterations);
            }

            reader.Dispose();

            Console.WriteLine();            
        }

        static void SaveNetwork(Network network, String savePath)
        {
            Console.WriteLine("Saving to " + alterateFile + savePath + " ...");

            NetworkSaver saver = new NetworkSaver(alterateFile + savePath);
            saver.Save(network);
            saver.Dispose();

            alterateFile = alterateFile == 1 ? 2 : alterateFile;
        }

        static byte[] DataTo1D(byte[,] input)
        {
            byte[] output = new byte[input.Length];

            int k = 0;
            for (int i = 0; i < input.GetLength(0); i++)
            {
                for (int j = 0; j < input.GetLength(1); j++)
                {
                    output[k] = input[i, j];
                    k++;
                }
            }

            return output;
        }
    }
}
