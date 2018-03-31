using System;

using ozmanet.neural_network;
using ozmanet.util;

namespace console
{
    class Program
    {
        static void Main(string[] args)
        {
            int[] layerSettings = { 784, 30, 10 };
            Network network = new Network(layerSettings);

            
            for (int run = 0; run < 10000; run++)
            {
                MnistReader reader = new MnistReader(
                    "../../../../data/digits/training/train-labels.idx1-ubyte",     // path for labels
                    "../../../../data/digits/training/train-images.idx3-ubyte");    // path for imgs

                int iterations = 0;
                int hit = 0;
                while (reader.HasNext() && iterations < 15000)
                {
                    CharacterImage input = reader.ReadNext();

                    byte[] dataB = DataTo1D(input.Data);
                    float[] dataF = new float[dataB.Length];

                    for (int i = 0; i < dataB.Length; i++)
                    {
                        dataF[i] = ((float)dataB[i]) / 255f;
                    }

                    float[] expected = new float[10];
                    int value = input.Value - '0';
                    expected[value] = 1f;

                    network.FeedForward(dataF);
                    network.Backpropagate(expected);

                    Console.Write("\r Cost: " + Math.Round(network.cost) + " Hits: " + hit + " / " + iterations);
                    iterations++;
                    if (value == network.actual)
                    {
                        hit++;
                    }
                    //Console.WriteLine("Expected: " + value + " ---- Actual: " + network.actual);
                    //Console.Clear();
                    //Console.ReadKey();
                }

                reader.Dispose();
            }

            /*
            while(true)
            {
                float[] input = { 1, 0 };
                float[] expected = { 0, 1 };

                network.FeedForward(input);
                network.Backpropagate(expected);

                Console.WriteLine("Expected: " + 1 + " ---- Actual: " + network.actual);

                float[] input2 = { 0, 1 };
                float[] expected2 = { 1, 0 };

                network.FeedForward(input2);
                network.Backpropagate(expected2);

                Console.WriteLine("Expected: " + 0 + " ---- Actual: " + network.actual);

                float[] input3 = { 1, 1 };
                float[] expected3 = { 0, 0 };

                network.FeedForward(input3);
                network.Backpropagate(expected3);

                Console.WriteLine("Expected: " + -1 + " ---- Actual: " + network.actual);
                Console.ReadKey();
             }
            */
           

                Console.Write("Press any key to end");
            Console.ReadKey();
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
