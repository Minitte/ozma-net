using System;
using System.Collections.Generic;
using System.Text;

namespace alexnet.neuralnetwork
{
    /// <summary>
    /// Settings for the network
    /// </summary>
    public class NetworkSettings
    {
        /// <summary>
        /// the layout of the layers
        /// </summary>
        private List<int> m_layout;

        public NetworkSettings(int inputNeurons, int[] hiddenNeurons, int outputNeurons)
        {
            m_layout = new List<int>();
            m_layout.Add(inputNeurons);
            m_layout.AddRange(hiddenNeurons);
            m_layout.Add(outputNeurons);
        }

        /// <summary>
        /// Adds a hidden layer to the end of the hidden layers
        /// </summary>
        /// <param name="neurons"></param>
        public void AddHiddenLayer(int neurons)
        {
            m_layout.Insert(m_layout.Count - 1, neurons);
        }

        /// <summary>
        /// Returns the sum of neurons currently in the layout
        /// </summary>
        /// <returns></returns>
        public int TotalNeurons()
        {
            int sum = 0;

            foreach (int i in m_layout)
            {
                sum += i;
            }

            return sum;
        }

        /// <summary>
        /// Returns the layer settings as an array
        /// </summary>
        /// <returns></returns>
        public int[] ToArray()
        {
            return m_layout.ToArray();
        }

    }
}
