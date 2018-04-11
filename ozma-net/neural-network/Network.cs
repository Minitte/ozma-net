using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ozmanet.neural_network
{
    public class Network
    {
        /**
         * Learning rate
         */
        private static double learningRate = 1.0;

        /**
         * Sum of all output neuron errors
         */
        private double cost;

        /**
         * Total number of hits for the current run
         */
        private int totalHits;

        /// <summary>
        /// Network Layout
        /// </summary>
        private int[] m_layout;

        /// <summary>
        /// Number of data this network has trained with
        /// </summary>
        private long m_trainCount;

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

        /**
         * Getter for cost 
         */
        public double Cost { get { return cost; } }

        /**
         * Getter for total hits
         */
        public int TotalHits { get { return totalHits; } set { totalHits = value; } }

        /// <summary>
        /// Network Layout
        /// </summary>
        public int[] Layout
        {
            get
            {
                return m_layout;
            }
        }

        /// <summary>
        /// Number of data this network has trained with
        /// </summary>
        public long TrainCount
        {
            get
            {
                return m_trainCount;
            }

            set
            {
                m_trainCount = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="numNeurons">number of neurons per layer, must be atleast 3 layers!</param>
        public Network(int[] numNeurons)
        {
            m_layout = numNeurons;

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
         * Feeds forward and backpropagates the sets of inputs and expected outputs.
         * Weights are adjusted after dividing by the total number of sets
         */
        public void Learn(float[,] inputs, float[,] expecteds)
        {
            if (inputs.GetLength(0) != expecteds.GetLength(0))
            {
                Console.WriteLine("Sets are uneven");
                return;
            }

            m_trainCount += inputs.GetLength(0);

            double numSets = inputs.GetLength(0);
            int floatSize = 4;

            for (int i = 0; i < inputs.GetLength(0); i++)
            {
                float[] input = new float[inputs.GetLength(1)];
                float[] expected = new float[expecteds.GetLength(1)];

                // Get the current rows
                Buffer.BlockCopy(inputs, floatSize * inputs.GetLength(1) * i, input, 0, floatSize * inputs.GetLength(1));
                Buffer.BlockCopy(expecteds, floatSize * expecteds.GetLength(1) * i, expected, 0, floatSize * expecteds.GetLength(1));

                int expectedNum = -2;

                for (int j = 0; j < expected.Length; j++)
                {
                    if (expected[j] == 1)
                    {
                        expectedNum = j;
                    }
                }

                if (FeedForward(input) == expectedNum)
                {
                    totalHits++;
                }

                Backpropagate(expected);
            }


            // Adjust hidden layer link weights
            Parallel.For(0, m_layers.Length - 1, k => //(int k = 1; k < m_layers.Length - 3; k++)
            {
                Parallel.For(0, m_layers[k].Neurons.Length, i => //(int i = 0; i < m_layers[m_layers.Length - 2].Neurons.Length; i++)
                {
                    Parallel.For(0, m_layers[k + 1].Neurons.Length, j => //(int j = 0; j < m_outputLayer.Neurons.Length; j++)
                    {
                        m_layers[k].Links[i, j].Weight +=
                            learningRate * m_layers[k].Links[i, j].Delta / numSets;

                        m_layers[k].Links[i, j].Delta = 0.0;
                    });
                });
            });

            // Adjust hidden layer link weights
            Parallel.For(1, m_layers.Length, k => //(int k = 1; k < m_layers.Length - 3; k++)
            {
                // Adjust hidden layer bias weights
                Parallel.For(0, m_layers[k].Neurons.Length, i =>//(int i = 0; i < m_layers[m_layers.Length - 2].Neurons.Length; i++)
                {
                    m_layers[k].Neurons[i].Bias += learningRate * m_layers[k].Neurons[i].Delta / numSets;
                    m_layers[k].Neurons[i].Delta = 0.0;
                });
            });
        }

        /**
         * Passes the input values forward through the network.
         */
        public int FeedForward(float[] inputs)
        {
            // Number of input values should be equal to number of input neurons
            if (inputs.Length != m_inputLayer.Neurons.Length)
            {
                Console.WriteLine("Invalid input");
                return -3;
            }

            //Reset nets
            Parallel.For(0, m_layers[m_layers.Length - 2].Neurons.Length, i => //(int i = 0; i < m_layers[m_layers.Length - 2].Neurons.Length; i++)
            {
                m_layers[m_layers.Length - 2].Neurons[i].Net = 0.0f;
            });

            Parallel.For(0, m_outputLayer.Neurons.Length, i => //(int i = 0; i < m_outputLayer.Neurons.Length; i++)
            {
                m_outputLayer.Neurons[i].Net = 0.0f;
            });

            // Apply the input values
            Parallel.For(0, inputs.Length, i => //(int i = 0; i < inputs.Length; i++)
            {
                m_inputLayer.Neurons[i].Out = inputs[i];
            });

            // Input layer to hidden layer
            m_inputLayer.UpdateNeuronNets();

            // Feed forward
            Parallel.For(1, m_layers.Length - 1, i => //(int i = 1; i < m_layers.Length - 1; i++)
            {
                m_layers[i].UpdateNeuronOuts();
                m_layers[i].UpdateNeuronNets();
            });

            // Activation function on output layer
            Parallel.For(0, m_outputLayer.Neurons.Length, i => //(int i = 0; i < m_outputLayer.Neurons.Length; i++)
            {
                m_outputLayer.Neurons[i].UpdateOut();
            });

            double max = -1.0;
            int actual = -1;

            for (int i = 0; i < m_outputLayer.Neurons.Length; i++)
            {
                if (m_outputLayer.Neurons[i].Out > max)
                {
                    max = m_outputLayer.Neurons[i].Out;
                    actual = i;
                }
            }

            // Return the value
            return max > 0.1 ? actual : -2;
        }

        public void Backpropagate(float[] expected)
        {
            // Number of expected values should be equal to number of output neurons
            if (expected.Length != m_outputLayer.Neurons.Length)
            {
                Console.WriteLine("Invalid expected");
                return;
            }

            // Calculate cost and errors
            double[] error = new double[expected.Length];
            cost = 0.0f;

            for (int i = 0; i < expected.Length; i++)
            {
                error[i] = Math.Pow((expected[i] - m_outputLayer.Neurons[i].Out), 2);
                cost += error[i];
            }

            double[] errorToNet = new double[expected.Length];

            // Calculate hidden layer to output layer deltas
            Parallel.For(0, expected.Length, i => //(int i = 0; i < expected.Length; i++)
            {
                errorToNet[i] = 2.0 * (expected[i] - m_outputLayer.Neurons[i].Out)
                    * util.MathF.SigmoidPrimes(m_outputLayer.Neurons[i].Net);

                m_outputLayer.Neurons[i].Delta += errorToNet[i];

                Parallel.For(0, m_layers[m_layers.Length - 2].Neurons.Length, j => //(int j = 0; j < m_layers[m_layers.Length - 2].Neurons.Length; j++)
                {
                    m_layers[m_layers.Length - 2].Links[j, i].Delta += errorToNet[i]
                        * m_layers[m_layers.Length - 2].Neurons[j].Out;
                });
            });

            // Initial derivative sum calculations
            Parallel.For(0, m_layers[m_layers.Length - 2].Neurons.Length, j =>
            {
                double sum = 0.0;
                Parallel.For(0, m_layers[m_layers.Length - 1].Neurons.Length, l =>
                {
                    sum += errorToNet[l] * m_layers[m_layers.Length - 2].Links[j, l].Weight;
                });

                m_layers[m_layers.Length - 2].Neurons[j].DerivativeSum = sum * util.MathF.SigmoidPrimes(m_layers[m_layers.Length - 2].Neurons[j].Net);
                
            });

            // Backpropagate through each layer
            for (int i = m_layers.Length - 2; i > 0; i--)
            {
                for (int j = 0; j < m_layers[i - 1].Neurons.Length; j++) //Parallel.For(0, m_layers[i - 1].Neurons.Length, j =>
                {
                    double sum = 0.0;
                    double sumWeighted = 0.0;
                    for (int k = 0; k < m_layers[i].Neurons.Length; k++) //Parallel.For(0, m_layers[i].Neurons.Length, k =>
                    {
                        m_layers[i - 1].Links[j, k].Delta += m_layers[i].Neurons[k].DerivativeSum
                            * m_layers[i - 1].Neurons[j].Out;

                        sum += m_layers[i].Neurons[k].DerivativeSum;
                        sumWeighted += m_layers[i].Neurons[k].DerivativeSum * m_layers[i - 1].Links[j, k].Weight;
                    }

                    m_layers[i - 1].Neurons[j].DerivativeSum = sumWeighted * util.MathF.SigmoidPrimes(m_layers[i - 1].Neurons[j].Net);
                    m_layers[i - 1].Neurons[j].Delta += sum;
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
