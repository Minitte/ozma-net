using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpf_ozma_net.neural_network
{
    class NeuronLayer
    {
        /// <summary>
        /// List of neurons in the layer
        /// </summary>
        private Neuron[] m_neurons;

        /// <summary>
        /// List of neurons in the layer
        /// </summary>
        public Neuron[] Neurons
        {
            get
            {
                return m_neurons;
            }
        }

        public NeuronLayer(Neuron[] neurons)
        {
            this.m_neurons = neurons;
        }
    }
}
