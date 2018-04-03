using System;
using System.Collections.Generic;
using System.Text;

namespace ozmanet.neural_network
{
    public class NeuronLink
    {
        private Neuron m_start, m_end;
        private double m_weight;

        private double delta;

        /// <summary>
        /// Left neuron
        /// </summary>
        public Neuron Start
        {
            get
            {
                return m_start;
            }
        }

        /// <summary>
        /// right neuron
        /// </summary>
        public Neuron End
        {
            get
            {
                return m_end;
            }
        }

        /// <summary>
        /// weight associated with the link
        /// </summary>
        public double Weight
        {
            get
            {
                return m_weight;
            }

            set
            {
                m_weight = value;
            }
        }

        public double Delta { get { return delta; } set { delta = value; } }

        public NeuronLink(Neuron start, Neuron end, float weight)
        {
            this.m_start = start;
            this.m_end = end;
            this.m_weight = weight;
        }
    }
}
