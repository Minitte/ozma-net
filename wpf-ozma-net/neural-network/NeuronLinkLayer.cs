using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpf_ozma_net.neural_network
{
    public class NeuronLinkLayer
    {
        /// <summary>
        /// List of neurons in the layer
        /// </summary>
        private Neuron[] m_neurons;

        /// <summary>
        /// List of (foward) links 
        /// </summary>
        private NeuronLink[,] m_links;

        /// <summary>
        /// List of neurons in the layer
        /// </summary>
        public Neuron[] Neurons
        {
            get
            {
                return m_neurons;
            }

            set
            {
                m_neurons = value;
            }
        }

        /// <summary>
        /// List of (foward) links 
        /// </summary>
        public NeuronLink[,] Links
        {
            get
            {
                return m_links;
            }

            set
            {
                m_links = value;
            }
        }

        public NeuronLinkLayer(Neuron[] neurons)
        {
            this.m_neurons = neurons;
        }

        /// <summary>
        /// gets the link when going backwards
        /// </summary>
        /// <param name="curIndex">the index of a neuron from the right side layer</param>
        /// <param name="backIndex">index of the desired neuron from this layer</param>
        /// <returns></returns>
        public NeuronLink BackwardGetLink(int curIndex, int backIndex)
        {
            return m_links[backIndex, curIndex];
        }

    }
}
