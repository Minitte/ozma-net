using System;

using ozmanet.neural_network;

namespace console
{
    class Program
    {
        static void Main(string[] args)
        {
            int[] layerSettings = { 1, 4, 1 };
            Network network = new Network(layerSettings);


            Console.Write("Press any key to end");
            Console.ReadKey();
        }
    }
}
