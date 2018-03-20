using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpf_ozma_net.neural_network
{
    class Network
    {

        /// <summary>
        /// List of layers in the network
        /// </summary>
        private NeuronLayer[] m_layers;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="numNeurons">number of neurons per layer</param>
        public Network(int[] numNeurons)
        {
            // create layers
            CreateLayers(numNeurons);

            // connect neurons
            SetupNeurons(10);
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

            m_layers = new NeuronLayer[numLayer];

            // create layers
            for (int l = 0; l < numLayer; l++) // l represents layer index
            {
                Neuron[] neurons = new Neuron[numNeurons[l]];

                // create bunch of neurons for that layer
                for (int n = 0; n < numNeurons[l]; n++) // n represents neuron index in the layer
                {
                    neurons[n] = new Neuron();
                }

                m_layers[l] = new NeuronLayer(neurons);
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
                // sets up each foward link
                for (int n = 0; n < m_layers[l].Neurons.Length; n++)
                {
                    // connecting current layer with next layer
                    m_layers[l].Neurons[n].FwdLink = new NeuronLink[m_layers[l + 1].Neurons.Length];
                }
            }

            // backwards link
            for (int l = 1; l < numLayer; l++)
            {
                // sets up each backwards link
                for (int n = 0; n < m_layers[l].Neurons.Length; n++)
                {
                    // connecting current layer with prev layer
                    m_layers[l].Neurons[n].BakLink = m_layers[l - 1].Neurons;
                }
            }
        }

        /// <summary>
        /// randomizes the weights 
        /// </summary>
        /// <param name="randSeed"></param>
        private void SetupWeights(int randSeed)
        {
            // random number generator
            Random rand = new Random(randSeed);

            // number of layers
            int numLayer = m_layers.Length;

            // for each layer
            for (int l = 0; l < numLayer; l++)
            {
                // sets up each neuron in the layer
                for (int n = 0; n < m_layers[l].Neurons.Length; n++)
                {
                    // setup weights
                    m_layers[l].Neurons[n].Weights = new float[m_layers[l + 1].Neurons.Length];

                    // random weights
                    for (int w = 0; w < m_layers[l + 1].Neurons.Length; w++)
                    {
                        m_layers[l].Neurons[n].Weights[w] = (float)rand.NextDouble();
                    }
                }
             }
    }
}
