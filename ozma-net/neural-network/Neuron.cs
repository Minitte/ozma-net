using System;
using System.Collections.Generic;
using System.Text;

namespace ozmanet.neural_network
{
    /// <summary>
    /// A class represneting a Neuron
    /// </summary>
    public class Neuron
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
