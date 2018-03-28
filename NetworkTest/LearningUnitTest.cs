using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ozmanet.neural_network;
using ozmanet.util;

namespace NetworkTest
{
    [TestClass]
    public class LearningUnitTest
    {
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

            reader.Dispose();

            Assert.AreEqual(0.00f, network.Layers[2].Neurons[0].Out, 0.01f);
            Assert.AreEqual(0.99f, network.Layers[2].Neurons[expected].Out, 0.01f);
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

            for (int i = 0; i < 300; i++)
            {
                network.Learn(inputSets, expectedSets);
            }

            reader.Dispose();

            Assert.AreEqual(0.01f, network.Layers[2].Neurons[0].Out, 0.05f);
            Assert.AreEqual(0.99f, network.Layers[2].Neurons[expected].Out, 0.05f);
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
            reader.Dispose();

            Assert.AreEqual(0.01f, network.Layers[2].Neurons[0].Out, 0.01f);
            Assert.AreEqual(0.99f, network.Layers[2].Neurons[expected].Out, 0.01f);
        }

        [TestMethod]
        public void TestLearnTwoNumbersSetsOfOne()
        {
            int[] layerSettings = { 784, 15, 10 };
            Network network = new Network(layerSettings);

            MnistReader reader = new MnistReader(
            "../../../data/digits/training/train-labels.idx1-ubyte",     // path for labels
            "../../../data/digits/training/train-images.idx3-ubyte");    // path for images

            // First number
            float[,] firstNumber = new float[1, 784];
            float[,] firstExpected = new float[1, 10];

            CharacterImage input = reader.ReadNext();
            byte[] dataBytes = input.DataTo1D();

            for (int i = 0; i < dataBytes.Length; i++)
            {
                firstNumber[0, i] = (float)dataBytes[i] / 255f;
            }

            int expected = input.Value - '0';

            firstExpected[0, expected] = 1f;

            // Second number
            float[,] secondNumber = new float[1, 784];
            float[,] secondExpected = new float[1, 10];

            input = reader.ReadNext();
            dataBytes = input.DataTo1D();

            for (int i = 0; i < dataBytes.Length; i++)
            {
                secondNumber[0, i] = (float)dataBytes[i] / 255f;
            }

            expected = input.Value - '0';

            secondExpected[0, expected] = 1f;

            for (int i = 0; i < 3000; i++)
            {
                network.Learn(firstNumber, firstExpected);
                network.Learn(secondNumber, secondExpected);
            }

            network.Learn(secondNumber, secondExpected);

            reader.Dispose();

            Assert.AreEqual(0.01f, network.Layers[2].Neurons[5].Out, 0.05f);
            Assert.AreEqual(0.99f, network.Layers[2].Neurons[0].Out, 0.05f);
        }
    }
}
