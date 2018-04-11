using System;
using System.Collections.Generic;
using System.Text;

namespace mnist_data_creator
{
    public class GeneralImage
    {
        /// <summary>
        /// image data
        /// </summary>
        private byte[,] m_image;

        /// <summary>
        /// label for the image
        /// </summary>
        private string m_label;

        /// <summary>
        /// label index or output index
        /// </summary>
        private byte m_labelIndex;

        /// <summary>
        /// Image data
        /// </summary>
        public byte[,] Image
        {
            get
            {
                return m_image;
            }

            set
            {
                m_image = value;
            }
        }

        /// <summary>
        /// Label for the image
        /// </summary>
        public string Label
        {
            get
            {
                return Label;
            }

            set
            {
                m_label = value;
            }
        }

        /// <summary>
        /// label index or output index
        /// </summary>
        public byte LabelIndex
        {
            get
            {
                return m_labelIndex;
            }

            set
            {
                m_labelIndex = value;
            }
        }

        public GeneralImage(byte[,] image, string label)
        {
            m_image = image;
            m_label = label;
        }

        public GeneralImage(byte[,] image, byte labelIndex)
        {
            m_image = image;
            m_labelIndex = labelIndex;
        }
    }
}
