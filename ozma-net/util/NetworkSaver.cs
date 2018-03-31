using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ozmanet.neural_network;

namespace ozmanet.util
{
    public class NetworkSaver
    {

        StreamWriter writer;

        public NetworkSaver(String path)
        {
            writer = new StreamWriter(new FileStream(path, FileMode.OpenOrCreate));
        }

        /// <summary>
        /// Saves the network's weights and baises to a file 
        /// </summary>
        /// <param name="net"></param>
        public void Save(Network net)
        {
            WriteHeader(net);
            Write(net);
        }

        /// <summary>
        /// Writes the layout of the network
        /// </summary>
        /// <param name="net"></param>
        private void WriteHeader(Network net)
        {
            // number of layers
            writer.WriteLine(net.Layout.Length);

            foreach (int n in net.Layout)
            {
                writer.WriteLine(n);
            }
        }

        /// <summary>
        /// Writes all of the neuron/link's weights and biases
        /// </summary>
        /// <param name="net"></param>
        private void Write(Network net)
        {
            // write each layer's contents
            foreach (NeuronLinkLayer layer in net.Layers)
            {
                // all neurons
                foreach (Neuron n in layer.Neurons)
                {
                    writer.WriteLine(n.Bias);
                }

                // all links
                for (int x = 0; x < layer.Links.GetLength(0); x++)
                {
                    for (int y = 0; y < layer.Links.GetLength(1); y++)
                    {
                        writer.WriteLine(layer.Links[x, y].Weight);
                    }
                }
            }
        }

        public void Dispose()
        {
            writer.Dispose();
        }
    }
}
