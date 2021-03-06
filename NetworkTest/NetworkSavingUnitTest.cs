﻿using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ozmanet.neural_network;
using ozmanet.util;

namespace NetworkTest
{
    /// <summary>
    /// Summary description for NetworkSavingUnitTest
    /// </summary>
    [TestClass]
    public class NetworkSavingUnitTest
    {

        [TestMethod]
        public void TestSaveLoad()
        {
            int[] layout = { 10, 32, 5 };
            String path = "tmptest.ozmanet";
            Network net1 = new Network(layout);
            net1.TrainCount = 5437543;

            NetworkSaver saver = new NetworkSaver(path);
            saver.Save(net1);
            saver.Dispose();

            NetworkLoader loader = new NetworkLoader(path);
            Network net2 = loader.Load();
            loader.Dispose();

            Assert.AreEqual(net1.TrainCount, net2.TrainCount);

            // test all layers to be the same
            for (int l = 0; l < net1.Layers.Length; l++)
            {
                NeuronLinkLayer layer1 = net1.Layers[l];
                NeuronLinkLayer layer2 = net2.Layers[l];

                Assert.AreEqual(layer1.Neurons.Length, layer2.Neurons.Length);

                // all neurons
                for (int n = 0; n < layer1.Neurons.Length; n++)
                {
                    Assert.AreEqual(layer1.Neurons[n].Bias, layer2.Neurons[n].Bias);
                }

                // all links
                if (l != net2.Layers.Length - 1)
                {
                    Assert.AreEqual(layer1.Links.GetLength(0), layer2.Links.GetLength(0));
                    Assert.AreEqual(layer1.Links.GetLength(1), layer2.Links.GetLength(1));

                    for (int x = 0; x < layer1.Links.GetLength(0); x++)
                    {
                        for (int y = 0; y < layer2.Links.GetLength(1); y++)
                        {
                            Assert.AreEqual(layer1.Links[x, y].Weight, layer2.Links[x, y].Weight, 0.000000001, String.Format("x:{0} y:{1}", x, y));
                        }
                    }
                }
            }
        }
    }
}
