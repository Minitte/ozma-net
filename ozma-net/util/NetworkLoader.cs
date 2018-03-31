using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ozmanet.neural_network;

namespace ozmanet.util
{
    public class NetworkLoader
    {
        private FileStream stream;
        private StreamReader reader;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">Path to a file containing a network</param>
        public NetworkLoader(String path)
        {
            stream = new FileStream(path, FileMode.Open);
            reader = new StreamReader(stream);
        }

        /// <summary>
        /// Reads the file and creates a network based on the values
        /// </summary>
        /// <returns></returns>
        public Network Load()
        {
            stream.Seek(0, SeekOrigin.Begin);

            int[] layout = ReadLayout();
            Network net = new Network(layout);

            ReadWeightsBiases(net);

            return net;
        }

        /// <summary>
        /// Reads the layout/header for the network
        /// </summary>
        /// <returns></returns>
        private int[] ReadLayout()
        {
            // read layout
            int numLayer = int.Parse(reader.ReadLine());

            // read neuron per layer
            int[] layout = new int[numLayer];
            for (int i = 0; i < layout.Length; i++)
            {
                layout[i] = int.Parse(reader.ReadLine());
            }

            return layout;
        }

        /// <summary>
        /// Adjust all of the weights and biases in the network
        /// </summary>
        /// <param name="net"></param>
        private void ReadWeightsBiases(Network net)
        {
            for (int l = 0; l < net.Layers.Length; l++)
            {
                NeuronLinkLayer layer = net.Layers[l];

                // all neurons
                foreach (Neuron n in layer.Neurons)
                {
                    n.Bias = double.Parse(reader.ReadLine());
                }

                // all links
                if (l != net.Layers.Length - 1)
                {
                    for (int x = 0; x < layer.Links.GetLength(0); x++)
                    {
                        for (int y = 0; y < layer.Links.GetLength(1); y++)
                        {
                            layer.Links[x, y].Weight = double.Parse(reader.ReadLine());
                        }
                    }
                }
            }
        }

        public void Dispose()
        {
            reader.Dispose();
            stream.Dispose();
        }
    }
}
