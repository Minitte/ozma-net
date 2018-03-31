using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ozmanet.neural_network;

namespace NetworkTest
{
    [TestClass]
    public class NetworkUnitTest
    {
        /*
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

        [TestMethod]
        public void TestSigmoidFunction()
        {
            // Test for sigmoid of zero
            float zero = 0.0f;
            float sigmoidZero = ozmanet.util.MathF.Sigmoid(zero);

            Assert.AreEqual(0.50f, sigmoidZero, 0.01f);

            // Test for sigmoid of one
            float one = 1.0f;
            float sigmoidOne = ozmanet.util.MathF.Sigmoid(one);

            Assert.AreEqual(0.73f, sigmoidOne, 0.01f);
        }

        [TestMethod]
        public void TestFeedForward()
        {
            int[] layerSettings = { 1, 2, 1 };
            Network network = new Network(layerSettings);

            NeuronLinkLayer[] layers = network.Layers;

            float inputToH1Weight = layers[0].Links[0, 0].Weight;
            float inputToH2Weight = layers[0].Links[0, 1].Weight;
            float[] inputValues = new float[] { 1.5f };

            network.FeedForward(inputValues);

            // Check hidden layer net values
            Assert.AreEqual(inputToH1Weight * inputValues[0], layers[1].Neurons[0].Net);
            Assert.AreEqual(inputToH2Weight * inputValues[0], layers[1].Neurons[1].Net);

            float H1ToOutputWeight = layers[1].Links[0, 0].Weight;
            float H2ToOutputWeight = layers[1].Links[1, 0].Weight;

            float H1Out = layers[1].Neurons[0].Out;
            float H2Out = layers[1].Neurons[1].Out;

            // Check output layer net value
            Assert.AreEqual(H1Out * H1ToOutputWeight + H2Out * H2ToOutputWeight, layers[2].Neurons[0].Net, 0.01f);
        }
        */
    }
}
