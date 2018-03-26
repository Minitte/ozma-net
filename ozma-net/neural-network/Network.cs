using System;
using System.Collections.Generic;
using System.Text;

namespace ozmanet.neural_network
{
    public class Network
    {
        private static float learningRate = 1.0f;

        /// <summary>
        /// List of layers in the network
        /// </summary>
        private NeuronLinkLayer[] m_layers;

        /// <summary>
        /// Layer responsible for input
        /// </summary>
        private NeuronLinkLayer m_inputLayer;

        /// <summary>
        /// Layer responsible for output
        /// </summary>
        private NeuronLinkLayer m_outputLayer;

        /// <summary>
        /// List of layers in the network
        /// </summary>
        public NeuronLinkLayer[] Layers
        {
            get
            {
                return m_layers;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="numNeurons">number of neurons per layer, must be atleast 3 layers!</param>
        public Network(int[] numNeurons)
        {
            // create layers
            CreateLayers(numNeurons);

            // input / output layer
            m_inputLayer = m_layers[0];
            m_outputLayer = m_layers[numNeurons.Length - 1];

            // connect neurons
            LinkNeurons();

            // randomize weights
            RandomizeWeights(10);
        }

        /**
         * Passes the input values forward through the network.
         */
        public void FeedForward(float[] inputs)
        {
            // Number of input values should be equal to number of input neurons
            if (inputs.Length != m_inputLayer.Neurons.Length)
            {
                Console.WriteLine("Invalid input");
                return;
            }

            // Apply the input values
            for (int i = 0; i < inputs.Length; i++)
            {
                m_inputLayer.Neurons[i].Out = inputs[i];
            }

            // Input layer to hidden layer
            m_inputLayer.UpdateNeuronNets();

            // Feed forward
            for (int i = 1; i < m_layers.Length - 1; i++)
            {
                m_layers[i].UpdateNeuronOuts();
                m_layers[i].UpdateNeuronNets();
            }
        }

        /**
         * Calculates the error for each output neuron.
         * Uses the formula: 1/2 * (expected - actual)^2
         * 
         * @return the error for each output neuron as a float array 
         */
        private float[] CalculateError(float[] expected)
        {
            float[] error = new float[expected.Length];

            for (int i = 0; i < expected.Length; i++)
            {
                error[i] = 1 / 2 * (float)Math.Pow((expected[i] - m_outputLayer.Neurons[i].Out), 2);
            }

            return error;
        }

        /**
         * Calculates the partial derivative of the error and the output of each output neuron.
         * Uses the formula: -2 * error
         * 
         * @return the partial derivatives as a float array 
         */
        private float[] CalculateErrorToOutput(float[] error)
        {
            float[] errorToOutput = new float[error.Length];

            for (int i = 0; i < error.Length; i++)
            {
                errorToOutput[i] = -2 * error[i];
            }

            return errorToOutput;
        }

        /**
         * Calculates the partial derivative of the output to net for each output neuron.
         * Uses the formula: out * (1 - out)
         * 
         * @return the partial derivatives as a float array
         */
        private float[] CalculateOutputToNet(int length)
        {
            float[] outputToNet = new float[length];

            for (int i = 0; i < length; i++)
            {
                outputToNet[i] = m_outputLayer.Neurons[i].Out * (1f - m_outputLayer.Neurons[i].Out);
            }

            return outputToNet;
        }

        /**
         * Calculates the partial derivative of each output neuron net value to its connected hidden outputs.
         * 
         * @return the partial derivatives as a 2d float array
         */
        private float[,] CalculateNetToWeight(int length)
        {
            //Hardcoded 1 to get hidden layer for now
            float[,] netToWeight = new float[length, m_layers[1].Neurons.Length];

            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < m_layers[1].Neurons.Length; j++)
                {
                    netToWeight[i, j] = m_layers[1].Neurons[j].Out;
                }
            }

            return netToWeight;
        }

        public void Backpropagate(float[] expected)
        {
            // Number of input values should be equal to number of input neurons
            if (expected.Length != m_outputLayer.Neurons.Length)
            {
                Console.WriteLine("Invalid expected");
                return;
            }

            float[] error = CalculateError(expected);

            float[] errorToOutput = CalculateErrorToOutput(error);

            float[] outputToNet = CalculateOutputToNet(m_outputLayer.Neurons.Length);

            float[,] netToWeight = CalculateNetToWeight(m_outputLayer.Neurons.Length);

            float[,] totalChange = new float[expected.Length, m_layers[1].Neurons.Length];

            for (int i = 0; i < expected.Length; i++)
            {
                for (int j = 0; j < m_layers[1].Neurons.Length; j++)
                {
                    totalChange[i, j] = errorToOutput[i] * outputToNet[i] * netToWeight[i, j];
                }
            }


        }

        /// <summary>
        /// Creates layers based on the given values
        /// None of the neurons are linked to each other
        /// </summary>
        /// <param name="numNeurons"></param>
        private void CreateLayers(int[] numNeurons)
        {
            // number of layers
            int numLayer = numNeurons.Length;

            m_layers = new NeuronLinkLayer[numLayer];

            // create layers
            for (int l = 0; l < numLayer; l++) // l represents layer index
            {
                Neuron[] neurons = new Neuron[numNeurons[l]];

                // create bunch of neurons for that layer
                for (int n = 0; n < numNeurons[l]; n++) // n represents neuron index in the layer
                {
                    neurons[n] = new Neuron();
                }

                m_layers[l] = new NeuronLinkLayer(neurons);
            }
        }

        /// <summary>
        /// Connects the layers and neurons together
        /// </summary>
        /// <param name="randSeed"></param>
        private void LinkNeurons()
        {
            // number of layers
            int numLayer = m_layers.Length;

            // connect layers
            for (int l = 0; l < numLayer - 1; l++)
            {
                NeuronLinkLayer leftLayer = m_layers[l];
                NeuronLinkLayer rightLayer = m_layers[l + 1];
                m_layers[l].Links = new NeuronLink[leftLayer.Neurons.Length, rightLayer.Neurons.Length];

                for (int leftN = 0; leftN < leftLayer.Neurons.Length; leftN++)
                {
                    for (int rightN = 0; rightN < rightLayer.Neurons.Length; rightN++)
                    {
                        m_layers[l].Links[leftN, rightN] = new NeuronLink(leftLayer.Neurons[leftN], rightLayer.Neurons[rightN], 0);
                    }
                }
            }
        }

        /// <summary>
        /// randomizes the weights 
        /// </summary>
        /// <param name="randSeed"></param>
        private void RandomizeWeights(int randSeed)
        {
            // random number generator
            Random rand = new Random(randSeed);

            // number of layers
            int numLayer = m_layers.Length;

            // for each layer
            for (int l = 0; l < numLayer - 1; l++)
            {
                NeuronLinkLayer leftLayer = m_layers[l];
                NeuronLinkLayer rightLayer = m_layers[l + 1];

                for (int leftN = 0; leftN < leftLayer.Neurons.Length; leftN++)
                {
                    for (int rightN = 0; rightN < rightLayer.Neurons.Length; rightN++)
                    {
                        m_layers[l].Links[leftN, rightN].Weight = (float)rand.NextDouble();
                    }
                }
            }
        }
    }
}
