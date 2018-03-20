using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpf_ozma_net.neural_network
{
    /// <summary>
    /// A class represneting a Neuron
    /// </summary>
    class Neuron
    {
        /// <summary>
        /// list of connected neurons
        /// </summary>
        private Neuron[] m_connected;

        /// <summary>
        /// Weights for each connected neuron
        /// </summary>
        private float[] m_weights;

        /// <summary>
        /// list of connected neurons
        /// </summary>
        public Neuron[] Connected
        {
            get
            {
                return m_connected;
            }
        }

        /// <summary>
        /// Weights for each connected neuron
        /// </summary>
        public float[] Weights
        {
            get
            {
                return m_weights;
            }
        }

        /// <summary>
        /// Constructor for a neuron
        /// </summary>
        /// <param name="connected"></param>
        /// <param name="weights"></param>
        public Neuron(Neuron[] connected, float[] weights)
        {
            this.m_connected = connected;
            this.m_weights = weights;
        }
    }
}
