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
        /// an incoming value held by the neuron
        /// </summary>
        private float net_value;

        /// <summary>
        /// an outgoing value held by the neuron
        /// </summary>
        private float out_value;

        private float bias;

        public float Net
        {
            get
            {
                return net_value;
            }

            set
            {
                net_value = value;
            }
        }

        public float Out
        {
            get
            {
                return out_value;
            }

            set
            {
                out_value = value;
            }
        }

        public float Bias { get { return bias; } set { bias = value; } }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Neuron()
        {
        }

        /**
         * Updates the out value of this neuron.
         * @return the out value
         */
        public float UpdateOut()
        {
            out_value = util.MathF.Sigmoid(net_value);
            return out_value;
        }
    }
}
