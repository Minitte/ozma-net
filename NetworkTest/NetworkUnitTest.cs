using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ozmanet.neural_network;

namespace NetworkTest
{
    [TestClass]
    public class NetworkUnitTest
    {

        [TestMethod]
        public void TestNetworkSetup()
        {
            int[] layerSettings = { 1, 4, 1 };
            Network network = new Network(layerSettings);

            NeuronLinkLayer[] layers = network.Layers;

            Assert.AreEqual(layers.Length == layerSettings.Length, String.Format("Found {0} layers instead of {2}", layers.Length, layerSettings.Length));

            for (int i = 0; i < layerSettings.Length; i++)
            {
                Assert.AreEqual(layers[i].Neurons.Length == layerSettings[i], String.Format("layer {0} has {1} neurons instead of {2}!", i, layers[i].Neurons.Length, layerSettings[i]));
            }
        }
    }
}
