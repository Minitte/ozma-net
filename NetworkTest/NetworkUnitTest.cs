using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ozmanet.neural_network;
using ozmanet.util;

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

        [TestMethod]
        public void TestBackpropagation()
        {
            int[] layerSettings = { 2, 2, 2 };
            Network network = new Network(layerSettings);

            network.Layers[0].Links[0, 0].Weight = 0.15f;
            network.Layers[0].Links[0, 1].Weight = 0.25f;
            network.Layers[0].Links[1, 0].Weight = 0.20f;
            network.Layers[0].Links[1, 1].Weight = 0.30f;

            network.Layers[1].Links[0, 0].Weight = 0.40f;
            network.Layers[1].Links[0, 1].Weight = 0.50f;
            network.Layers[1].Links[1, 0].Weight = 0.45f;
            network.Layers[1].Links[1, 1].Weight = 0.55f;

            float[] input = new float[] { 0.05f, 0.10f };
            float[] expected = new float[] { 0.01f, 0.99f };

            for (int i = 0; i < 10000; i++)
            {
                network.FeedForward(input);
                network.Backpropagate(expected);
            }

            // Check if the network learned
            Assert.AreEqual(0.01f, network.Layers[2].Neurons[0].Out, 0.01f);
            Assert.AreEqual(0.99f, network.Layers[2].Neurons[1].Out, 0.01f);
        }

        [TestMethod]
        public void TestLearnOneNumber()
        {
            int[] layerSettings = { 784, 15, 10 };
            Network network = new Network(layerSettings);

            MnistReader reader = new MnistReader(
                "../../../data/digits/training/train-labels.idx1-ubyte",     // path for labels
                "../../../data/digits/training/train-images.idx3-ubyte");    // path for images

            CharacterImage input = reader.ReadNext();
            byte[] datab = input.DataTo1D();
            float[] dataf = new float[datab.Length];
            if (dataf.Length != 28 * 28)
            {
                Console.WriteLine("");
            }

            for (int i = 0; i < datab.Length; i++)
            {
                dataf[i] = (float)datab[i] / 255f;
            }

            int expected = input.Value - '0';
            float[] expectedArray = new float[10];

            expectedArray[expected] = 1;

            int actual = network.FeedForward(dataf);

            reader.Dispose();

            for (int i = 0; i < 10000; i++)
            {
                network.FeedForward(dataf);
                network.Backpropagate(expectedArray);
            }

            // Check if the network learned
            Assert.AreEqual(0.00f, network.Layers[2].Neurons[0].Out, 0.01f);
            Assert.AreEqual(0.99f, network.Layers[2].Neurons[expected].Out, 0.01f);

            reader.Dispose();
        }

        [TestMethod]
        public void TestLearnOneNumberSetsOfOne()
        {
            int[] layerSettings = { 784, 15, 10 };
            Network network = new Network(layerSettings);

            MnistReader reader = new MnistReader(
            "../../../data/digits/training/train-labels.idx1-ubyte",     // path for labels
            "../../../data/digits/training/train-images.idx3-ubyte");    // path for images

            float[,] inputSets = new float[1, 784];
            float[,] expectedSets = new float[1, 10];

            CharacterImage input = reader.ReadNext();
            byte[] dataBytes = input.DataTo1D();

            for (int i = 0; i < dataBytes.Length; i++)
            {
                inputSets[0, i] = (float)dataBytes[i] / 255f;
            }

            int expected = input.Value - '0';

            expectedSets[0, expected] = 1f;

            for (int i = 0; i < 1000; i++)
            {
                network.Learn(inputSets, expectedSets);
            }

            Assert.AreEqual(0.00f, network.Layers[2].Neurons[0].Out, 0.01f);
            Assert.AreEqual(0.99f, network.Layers[2].Neurons[expected].Out, 0.01f);

            reader.Dispose();
        }

        [TestMethod]
        public void TestLearnOneNumberSetsOfFifty()
        {
            int[] layerSettings = { 784, 15, 10 };
            Network network = new Network(layerSettings);

            MnistReader reader = new MnistReader(
            "../../../data/digits/training/train-labels.idx1-ubyte",     // path for labels
            "../../../data/digits/training/train-images.idx3-ubyte");    // path for images

            float[,] inputSets = new float[50, 784];
            float[,] expectedSets = new float[50, 10];

            CharacterImage input = reader.ReadNext();
            byte[] dataBytes = input.DataTo1D();

            for (int i = 0; i < dataBytes.Length; i++)
            {
                for (int j = 0; j < 50; j++)
                {
                    inputSets[j, i] = (float)dataBytes[i] / 255f;
                }
            }

            int expected = input.Value - '0';

            for (int i = 0; i < 50; i++)
            {
                expectedSets[i, expected] = 1f;
            }

            for (int i = 0; i < 500; i++)
            {
                network.Learn(inputSets, expectedSets);
            }

            Assert.AreEqual(0.01f, network.Layers[2].Neurons[0].Out, 0.01f);
            Assert.AreEqual(0.99f, network.Layers[2].Neurons[expected].Out, 0.01f);

            reader.Dispose();
        }

        [TestMethod]
        public void TestLearnOneNumberDifferentInputs()
        {
            int[] layerSettings = { 784, 15, 10 };
            Network network = new Network(layerSettings);

            MnistReader reader = new MnistReader(
            "../../../data/digits/training/train-labels.idx1-ubyte",     // path for labels
            "../../../data/digits/training/train-images.idx3-ubyte");    // path for images

            float[,] inputSets = new float[10, 784];
            float[,] expectedSets = new float[10, 10];

            CharacterImage input = reader.ReadNext();
            byte[] dataBytes = input.DataTo1D();

            for (int i = 0; i < dataBytes.Length; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    inputSets[j, i] = (float)dataBytes[i] / 255f;
                }
            }

            int expected = input.Value - '0';

            for (int i = 0; i < 10; i++)
            {
                expectedSets[i, expected] = 1f;
            }

            while (reader.HasNext())
            {
                CharacterImage inputB = reader.ReadNext();

                int val = inputB.Value - '0';
                if (val == expected)
                {
                    byte[] dataBytesB = inputB.DataTo1D();

                    for (int i = 0; i < dataBytesB.Length; i++)
                    {
                        for (int j = 5; j < 10; j++)
                        {
                            inputSets[j, i] = (float)dataBytesB[i] / 255f;
                        }
                    }
                    break;
                }
            }

            for (int i = 0; i < 500; i++)
            {
                network.Learn(inputSets, expectedSets);
            }

            Assert.AreEqual(0.01f, network.Layers[2].Neurons[0].Out, 0.01f);
            Assert.AreEqual(0.99f, network.Layers[2].Neurons[expected].Out, 0.01f);

            reader.Dispose();
        }


    }
}
