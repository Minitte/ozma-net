using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpf_ozma_net.neural_network
{
    public class Network
    {

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
            for (int l = 0; l < numLayer; l++)
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
