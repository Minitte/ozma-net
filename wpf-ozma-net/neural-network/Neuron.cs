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
        /// a value held by the neuron
        /// </summary>
        private float m_value;

        public float Value
        {
            get
            {
                return m_value;
            }

            set
            {
                m_value = value;
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Neuron()
        {
        }
    }
}
