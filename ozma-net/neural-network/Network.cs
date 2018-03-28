using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ozmanet.neural_network
{
    public class Network
    {
        private static float learningRate = 1.0f;
        public float cost;

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

        public void Learn(float[,] inputs, float[,] expected)
        {
            // Should be equal amount of sets
            if (inputs.GetLength(0) != expected.GetLength(0))
            {
                Console.WriteLine("Invalid learning input");
                return;
            }

            Changes[] changes = new Changes[inputs.GetLength(0)];
            int floatSize = 4;
            int inputRowSize = inputs.GetLength(1);
            int expectedRowSize = expected.GetLength(1);

            // Feed in data one row at a time
            for (int i = 0; i < inputs.GetLength(0); i++)
            {
                float[] inputRow = new float[inputRowSize];
                Buffer.BlockCopy(inputs, inputRowSize * floatSize * i, inputRow, 0, floatSize * inputRowSize);
                FeedForward(inputRow);

                float[] expectedRow = new float[expectedRowSize];
                Buffer.BlockCopy(expected, expectedRowSize * floatSize * i, expectedRow, 0, floatSize * expectedRowSize);
                changes[i] = Backpropagate(expectedRow);
            }

            float[,] hiddenChanges = new float[m_outputLayer.Neurons.Length, m_layers[1].Neurons.Length];
            float[,] inputChanges = new float[m_layers[1].Neurons.Length, m_inputLayer.Neurons.Length];
            float[] outputBiasChanges = new float[m_outputLayer.Neurons.Length];
            float[] hiddenBiasChanges = new float[m_layers[1].Neurons.Length];

            for (int i = 0; i < changes.Length; i++)
            {
                // Sum of change to each weight in hidden layer
                for (int j = 0; j < changes[i].OutputToHiddenChanges.GetLength(1); j++)
                {
                    for (int k = 0; k < changes[i].OutputToHiddenChanges.GetLength(0); k++)
                    {
                        hiddenChanges[k, j] += changes[i].OutputToHiddenChanges[k, j];
                    }
                }

                // Sum of change to each weight in input layer
                for (int j = 0; j < changes[i].HiddenToInputChanges.GetLength(1); j++)
                {
                    for (int k = 0; k < changes[i].HiddenToInputChanges.GetLength(0); k++)
                    {
                        inputChanges[k, j] += changes[i].HiddenToInputChanges[k, j];
                    }
                }

                // Sum of change to each bias in output layer
                for (int j = 0; j < changes[i].OutputBiasChanges.Length; j++)
                {
                    for (int k = 0; k < changes[i].OutputBiasChanges.Length; k++)
                    {
                        outputBiasChanges[j] += changes[j].OutputBiasChanges[k];
                    }
                }

                // Sum of change to each bias in hidden layer
                for (int j = 0; j < changes[i].HiddenBiasChanges.Length; j++)
                {
                    for (int k = 0; k < changes[i].HiddenBiasChanges.Length; k++)
                    {
                        hiddenBiasChanges[j] += changes[j].HiddenBiasChanges[k];
                    }
                }
            }

            // Apply changes
            for (int i = 0; i < hiddenChanges.GetLength(0); i++)
            {
                for (int j = 0; j < hiddenChanges.GetLength(1); j++)
                {
                    hiddenChanges[i, j] /= changes.Length;
                    m_layers[1].Links[j, i].Weight -= learningRate * hiddenChanges[i, j];
                }
            }


            for (int i = 0; i < inputChanges.GetLength(0); i++)
            {
                for (int j = 0; j < inputChanges.GetLength(1); j++)
                {
                    inputChanges[i, j] /= changes.Length;
                    m_inputLayer.Links[j, i].Weight -= learningRate * inputChanges[i, j];
                }
            }

            for (int i = 0; i < outputBiasChanges.Length; i++)
            {
                m_outputLayer.Neurons[i].Bias -= learningRate * outputBiasChanges[i] / changes.Length;
            }

            for (int i = 0; i < hiddenBiasChanges.Length; i++)
            {
                m_layers[1].Neurons[i].Bias -= learningRate * hiddenBiasChanges[i] / changes.Length;
            }

            //Console.WriteLine("OZMA");
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
                return -1;
            }

            ResetNets();

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

            // Update output values
            for (int i = 0; i < m_outputLayer.Neurons.Length; i++)
            {
                m_outputLayer.Neurons[i].UpdateOut();
            }

            float max = 0.0f;
            int index = -1;

            for (int i = 0; i < m_outputLayer.Neurons.Length; i++)
            {
                if (m_outputLayer.Neurons[i].Out > max)
                {
                    max = m_outputLayer.Neurons[i].Out;
                    index = i;
                }


            }
            if (max > 0.9f)
            {
                return index;
            }
            else
            {
                return -2;
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
            float sumError = 0.0f;

            for (int i = 0; i < expected.Length; i++)
            {
                error[i] = 1f / 2 * (float)Math.Pow((expected[i] - m_outputLayer.Neurons[i].Out), 2);
                sumError += error[i];
            }

            cost = sumError; //Update cost

            return error;
        }

        /**
         * Calculates the partial derivative of the error and the output of each output neuron.
         * 
         * @return the partial derivatives as a float array 
         */
        private float[] DerivativeErrorToOutput(float[] expected)
        {
            float[] errorToOutput = new float[expected.Length];

            for (int i = 0; i < expected.Length; i++)
            {
                errorToOutput[i] = -(expected[i] - m_outputLayer.Neurons[i].Out);
            }

            return errorToOutput;
        }

        /**
         * Calculates the partial derivative of the output to net for each output neuron.
         * Uses the formula: out * (1 - out)
         * 
         * @return the partial derivatives as a float array
         */
        private float[] DerivativeOutToNet(NeuronLinkLayer layer)
        {
            float[] outToNet = new float[layer.Neurons.Length];

            for (int i = 0; i < layer.Neurons.Length; i++)
            {
                outToNet[i] = layer.Neurons[i].Out * (1f - layer.Neurons[i].Out);
            }

            return outToNet;
        }

        /**
         * Calculates the partial derivative of each output neuron net value to its connected hidden outputs.
         * 
         * @return the partial derivatives as a 2d float array
         */
        private float[,] DerivativeOutputNetToWeights()
        {
            //Hardcoded 1 to get hidden layer for now
            float[,] netToWeight = new float[m_outputLayer.Neurons.Length, m_layers[1].Neurons.Length];

            for (int i = 0; i < m_outputLayer.Neurons.Length; i++)
            {
                for (int j = 0; j < m_layers[1].Neurons.Length; j++)
                {
                    netToWeight[i, j] = m_layers[1].Neurons[j].Out;
                }
            }

            return netToWeight;
        }

        private void ResetNets()
        {
            for (int i = 0; i < m_layers.Length; i++)
            {
                for (int j = 0; j < m_layers[i].Neurons.Length; j++)
                {
                    m_layers[i].Neurons[j].Net = 0.0f;
                }
            }
        }

        /**
         * Performs backpropagation on the network.
         * Currently assumes that all neurons between two layers are fully connected.
         */
        public Changes Backpropagate(float[] expected)
        {
            // Number of input values should be equal to number of input neurons
            if (expected.Length != m_outputLayer.Neurons.Length)
            {
                Console.WriteLine("Invalid expected");
                return null;
            }

            float[] error = CalculateError(expected);


            // Output to hidden backpropagation

            float[] errorToOut = DerivativeErrorToOutput(expected);

            float[] outputToNet = DerivativeOutToNet(m_outputLayer);

            float[,] netToWeight = DerivativeOutputNetToWeights();

            // Output layer to hidden layer change
            float[,] outputToHiddenChange = new float[expected.Length, m_layers[1].Neurons.Length];

            for (int i = 0; i < expected.Length; i++)
            {
                for (int j = 0; j < m_layers[1].Neurons.Length; j++)
                {
                    outputToHiddenChange[i, j] = errorToOut[i] * outputToNet[i] * netToWeight[i, j];
                }
            }

            // Hidden to input backpropagation
            
            float[] errorToHiddenOut = new float[m_layers[1].Neurons.Length];

            for (int i = 0; i < m_layers[1].Neurons.Length; i++)
            {
                float sum = 0.0f;
                for (int j = 0; j < expected.Length; j++)
                {
                    sum += errorToOut[j] * outputToNet[j] * m_layers[1].Links[i, j].Weight;
                }
                errorToHiddenOut[i] = sum;
            }

            float[] hiddenOutToNet = new float[m_layers[1].Neurons.Length];
            
            for (int i = 0; i < m_layers[1].Neurons.Length; i++)
            {
                hiddenOutToNet[i] = util.MathF.SigmoidPrimes(m_layers[1].Neurons[i].Net);
            }

            // Hidden layer to input layer
            float[,] hiddenToInputChange = new float[m_layers[1].Neurons.Length, m_inputLayer.Neurons.Length];

            for (int i = 0; i < m_layers[1].Neurons.Length; i++)
            {
                for (int j = 0; j < m_inputLayer.Neurons.Length; j++)
                {
                    hiddenToInputChange[i, j] = errorToHiddenOut[i] * hiddenOutToNet[i] * m_inputLayer.Neurons[j].Out;
                }
            }

            // Bias changes
            float[] outputBiasChanges = new float[m_outputLayer.Neurons.Length];

            for (int i = 0; i < m_outputLayer.Neurons.Length; i++)
            {
                outputBiasChanges[i] = errorToOut[i] * outputToNet[i];
            }

            float[] hiddenBiasChanges = new float[m_layers[1].Neurons.Length];

            for (int i = 0; i < m_layers[1].Neurons.Length; i++)
            {
                for (int j = 0; j < m_outputLayer.Neurons.Length; j++)
                {
                    hiddenBiasChanges[i] += errorToOut[j] * outputToNet[j];
                }
            }

            return new Changes(outputToHiddenChange, hiddenToInputChange, outputBiasChanges, hiddenBiasChanges);
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
                        m_layers[l].Links[leftN, rightN].Weight = (float)rand.NextDouble() / 5;
                    }
                }
            }

            for (int i = 1; i < numLayer; i++)
            {
                for (int j = 0; j < m_layers[i].Neurons.Length; j++)
                {
                    m_layers[i].Neurons[j].Bias = (float)rand.NextDouble() * 2;
                }
            }
        }
    }

    /**
     * Class to represent proposed weight changes to the network
     */
    public class Changes
    {
        private float[,] outputToHiddenChanges;
        private float[,] hiddenToInputChanges;
        private float[] outputBiasChanges;
        private float[] hiddenBiasChanges;

        public float[,] OutputToHiddenChanges { get { return outputToHiddenChanges; } }
        public float[,] HiddenToInputChanges { get { return hiddenToInputChanges; } }
        public float[] OutputBiasChanges { get { return outputBiasChanges; } }
        public float[] HiddenBiasChanges { get { return hiddenBiasChanges; } }

        public Changes(float[,] outputToHiddenChanges, float[,] hiddenToInputChanges,
            float[] outputBiasChanges, float[] hiddenBiasChanges)
        {
            this.outputToHiddenChanges = outputToHiddenChanges;
            this.hiddenToInputChanges = hiddenToInputChanges;
            this.outputBiasChanges = outputBiasChanges;
            this.hiddenBiasChanges = hiddenBiasChanges;
        }
    }
}
