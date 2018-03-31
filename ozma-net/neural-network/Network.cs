using System;
using System.Collections.Generic;
using System.Text;

namespace ozmanet.neural_network
{
    public class Network
    {
        private static double learningRate = 1.0;
        public double actual;

        public double cost;

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

            //Reset nets
            for (int i = 0; i < m_layers[m_layers.Length - 2].Neurons.Length; i++)
            {
                m_layers[m_layers.Length - 2].Neurons[i].Net = 0.0f;
            }

            for (int i = 0; i < m_outputLayer.Neurons.Length; i++)
            {
                m_outputLayer.Neurons[i].Net = 0.0f;
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

            // Activation function on output layer
            for (int i = 0; i < m_outputLayer.Neurons.Length; i++)
            {
                m_outputLayer.Neurons[i].UpdateOut();
            }

            double max = -1.0;

            for (int i = 0; i < m_outputLayer.Neurons.Length; i++)
            {
                if (m_outputLayer.Neurons[i].Out > max)
                {
                    max = m_outputLayer.Neurons[i].Out;
                    actual = i;
                }
            }

            if (max < 0.5)
            {
                actual = -1;
            }
        }

        public void Backpropagate(float[] expected)
        {
            // Number of expected values should be equal to number of output neurons
            if (expected.Length != m_outputLayer.Neurons.Length)
            {
                Console.WriteLine("Invalid expected");
                return;
            }

            double[] error = new double[expected.Length];
            cost = 0.0f;

            for (int i = 0; i < expected.Length; i++)
            {
                error[i] = Math.Pow((expected[i] - m_outputLayer.Neurons[i].Out), 2);
                cost += error[i];
            }

            double[] errorToNet = new double[expected.Length];

            for (int i = 0; i < expected.Length; i++)
            {
                errorToNet[i] = 2.0 * (expected[i] - m_outputLayer.Neurons[i].Out)
                    * util.MathF.SigmoidPrimes(m_outputLayer.Neurons[i].Net);

                m_outputLayer.Neurons[i].Delta += errorToNet[i];

                for (int j = 0; j < m_layers[m_layers.Length - 2].Neurons.Length; j++)
                {
                    m_layers[m_layers.Length - 2].Links[j, i].Delta += errorToNet[i]
                        * m_layers[m_layers.Length - 2].Neurons[j].Out;
                }
            }

            for (int j = 0; j < m_layers[m_layers.Length - 2].Neurons.Length; j++)
            {
                double sum = 0.0;
                for (int l = 0; l < m_outputLayer.Neurons.Length; l++)
                {
                    sum += errorToNet[l] * m_layers[m_layers.Length - 2].Links[j, l].Weight;
                }

                for (int k = 0; k < m_inputLayer.Neurons.Length; k++)
                {
                    m_inputLayer.Links[k, j].Delta += sum
                        * util.MathF.SigmoidPrimes(m_layers[m_layers.Length - 2].Neurons[j].Net)
                        * m_inputLayer.Neurons[k].Out;

                    
                }
                m_layers[m_layers.Length - 2].Neurons[j].Delta += sum * util.MathF.SigmoidPrimes(m_layers[m_layers.Length - 2].Neurons[j].Net);
            }


            // Temp
            for (int k = 0; k < m_inputLayer.Neurons.Length; k++)
            {
                for (int i = 0; i < m_layers[m_layers.Length - 2].Neurons.Length; i++)
                {
                    m_inputLayer.Links[k, i].Weight +=
                        learningRate * m_inputLayer.Links[k, i].Delta;

                    m_inputLayer.Links[k, i].Delta = 0.0;
                }
            }
       
            for (int i = 0; i < m_layers[m_layers.Length - 2].Neurons.Length; i++)
            {
                for (int j = 0; j < m_outputLayer.Neurons.Length; j++)
                {
                    m_layers[m_layers.Length - 2].Links[i, j].Weight +=
                        learningRate * m_layers[m_layers.Length - 2].Links[i, j].Delta;

                    m_layers[m_layers.Length - 2].Links[i, j].Delta = 0.0;
                }
            }

            for (int i = 0; i < m_outputLayer.Neurons.Length; i++)
            {
                m_outputLayer.Neurons[i].Bias += learningRate * m_outputLayer.Neurons[i].Delta;
                m_outputLayer.Neurons[i].Delta = 0.0;
            }

            for (int i = 0; i < m_layers[m_layers.Length - 2].Neurons.Length; i++)
            {
                m_layers[m_layers.Length - 2].Neurons[i].Bias += learningRate * m_layers[m_layers.Length - 2].Neurons[i].Delta;
                m_layers[m_layers.Length - 2].Neurons[i].Delta = 0.0;
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
                        m_layers[l].Links[leftN, rightN].Weight = Math.Round(rand.NextDouble(), 1) / 10;
                    }
                }
            }

            for (int i = 1; i < numLayer; i++)
            {
                for (int j = 0; j < m_layers[i].Neurons.Length; j++)
                {
                    m_layers[i].Neurons[j].Bias = -Math.Round(rand.NextDouble(), 1);
                }

            }
        }
    }
}
