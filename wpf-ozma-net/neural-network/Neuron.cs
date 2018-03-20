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
        /// list of connected neurons going foward
        /// </summary>
        private NeuronLink[] m_fwdLink;

        /// <summary>
        /// list of connected neurons going backwards
        /// </summary>
        private NeuronLink[] m_bakLink;

        /// <summary>
        /// list of connected neurons going foward
        /// </summary>
        public NeuronLink[] FwdLink
        {
            get
            {
                return m_fwdLink;
            }

            set
            {
                m_fwdLink = value;
            }
        }

        /// <summary>
        /// list of connected neurons going backwards
        /// </summary>
        public NeuronLink[] BakLink
        {
            get
            {
                return m_bakLink;
            }

            set
            {
                m_bakLink = value;
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
