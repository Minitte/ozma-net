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
            "../../../../data/digits/training/train-images.idx3-ubyte");    // path for 

            while (reader.HasNext())
            {
                CharacterImage input = reader.ReadNext();
                byte[] dataBytes = input.DataTo1D();
                float[] dataFloats = new float[dataBytes.Length];

                for (int i = 0; i < dataBytes.Length; i++)
                {
                    dataFloats[i] = (float)dataBytes[i] / 255f;
                }

                int expected = input.Value - '0';
                float[] expectedArray = new float[10];

                expectedArray[expected] = 1f;

                int actual = network.FeedForward(dataFloats);

                Console.WriteLine("Expected: " + expected + " Actual: " + actual);

                /*
                if (expected == actual)
                {
                    Console.WriteLine("O Z M A O Z M A O Z M A O Z M A O Z M A O Z M A O Z M A");
                    Console.WriteLine("O Z M A O Z M A O Z M A O Z M A O Z M A O Z M A O Z M A");
                    Console.WriteLine("O Z M A O Z M A O Z M A O Z M A O Z M A O Z M A O Z M A");
                    Console.WriteLine("O Z M A O Z M A O Z M A O Z M A O Z M A O Z M A O Z M A");
                }*/

                network.Backpropagate(expectedArray);
                //Console.ReadKey();
            }

            Console.Write("Press any key to end");
            Console.ReadKey();
        }
    }
}
