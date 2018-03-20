using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpf_ozma_net.neural_network
{
    class NeuronLinkLayer
    {
        /// <summary>
        /// List of neurons in the layer
        /// </summary>
        private Neuron[] m_neurons;

        /// <summary>
        /// List of (foward) links 
        /// </summary>
        private NeuronLink[] m_links;

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
        public NeuronLink[] Links
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
    }
}
