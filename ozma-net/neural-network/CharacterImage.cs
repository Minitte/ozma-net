using System;
using System.Collections.Generic;
using System.Text;

namespace ozmanet.neural_network
{
    public class CharacterImage
    {
        private byte[,] m_data;
        private char m_value;

        /// <summary>
        /// image data
        /// </summary>
        public byte[,] Data
        {
            get
            {
                return m_data;
            }
        }

        /// <summary>
        /// char of the image (what the image represents)
        /// </summary>
        public char Value
        {
            get
            {
                return m_value;
            }
        }

        /// <summary>
        /// Constructor for an image that represents a character
        /// </summary>
        /// <param name="data">data of an image</param>
        /// <param name="value">value of the image</param>
        public CharacterImage(byte[,] data, char value)
        {
            m_data = data;
            m_value = value;
        }

        /**
         * Converts the 2d data array into 1d
         */
        public byte[] DataTo1D()
        {
            byte[] output = new byte[m_data.Length];

            int k = 0;
            for (int i = 0; i < m_data.GetLength(0); i++)
            {
                for (int j = 0; j < m_data.GetLength(1); j++)
                {
                    output[k] = m_data[i, j];
                    k++;
                }
            }

            return output;
        }
    }
}
