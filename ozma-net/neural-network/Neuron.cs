﻿using System;
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
        private double net_value;

        /// <summary>
        /// an outgoing value held by the neuron
        /// </summary>
        private double out_value;

        private double bias;

        private double delta;

        private double derivativeSum;

        public double Net
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

        public double Out
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

        public double Bias { get { return bias; } set { bias = value; } }

        public double Delta { get { return delta; } set { delta = value; } }

        public double DerivativeSum { get { return derivativeSum; } set { derivativeSum = value; } }

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
        public double UpdateOut()
        {
            net_value = net_value + bias;
            out_value = util.MathF.Sigmoid(net_value);
            return out_value;
        }
    }
}
