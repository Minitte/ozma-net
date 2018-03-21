using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ozmanet.neural_network;

namespace NetworkTest
{
    [TestClass]
    public class NetworkUnitTest
    {

        [TestMethod]
        public void TestLinkReferences()
        {
            int[] layerSettings = { 1, 3, 1 };
            Network network = new Network(layerSettings);
            NeuronLinkLayer[] layers = network.Layers;

            // all links
            for (int l = 0; l < layers.Length - 1; l++)
            {
                NeuronLinkLayer left = layers[l];
                NeuronLinkLayer right = layers[l + 1];

                for (int ln = 0; ln < left.Neurons.Length; ln++)
                {
                    for (int rn = 0; rn < right.Neurons.Length; rn++)
                    {
                        Assert.AreEqual(left.Links[ln, rn].Start, left.Neurons[ln]);
                        Assert.AreEqual(left.Links[ln, rn].End, right.Neurons[rn]);
                    }
                }
            }
        }

        [TestMethod]
        public void TestNetworkSetup()
        {
            int[] layerSettings = { 1, 4, 1 };
            Network network = new Network(layerSettings);

            NeuronLinkLayer[] layers = network.Layers;

            Assert.AreEqual(layers.Length, layerSettings.Length, String.Format("Found {0} layers instead of {1}", layers.Length, layerSettings.Length));

            for (int i = 0; i < layerSettings.Length; i++)
            {
                Assert.AreEqual(layers[i].Neurons.Length, layerSettings[i], String.Format("layer {0} has {1} neurons instead of {2}!", i, layers[i].Neurons.Length, layerSettings[i]));
            }
        }
    }
}
