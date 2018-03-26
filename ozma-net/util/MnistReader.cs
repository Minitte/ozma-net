using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ozmanet.neural_network;

namespace ozmanet.util
{
    public class MnistReader
    {
        private BinaryReader m_labelReader;
        private BinaryReader m_imgReader;
        private int m_imgWidth, m_imgHeight;
        private int m_numLabels, m_numImgs; // these should be the same
        private int m_readIndex = 0;

        /// <summary>
        /// Constructor for a mnist data reader
        /// </summary>
        /// <param name="labelPath">path to a mnist file containing the labels</param>
        /// <param name="imgPath">path to a mnist file containing the images</param>
        public MnistReader(String labelPath, String imgPath) 
        {
            m_labelReader = new BinaryReader(new FileStream(labelPath, FileMode.Open));
            m_imgReader = new BinaryReader(new FileStream(imgPath, FileMode.Open));
        }

        /// <summary>
        /// Reads the header for data
        /// </summary>
        private void ReadHeader() 
        {
            // header for labels
            m_labelReader.ReadInt32(); // discard first
            m_numLabels = m_labelReader.ReadInt32();

            // header for imgs
            m_imgReader.ReadInt32(); // discard first
            m_numImgs = m_imgReader.ReadInt32();
            m_imgWidth = m_imgReader.ReadInt32();
            m_imgHeight = m_imgReader.ReadInt32();
        }

        /// <summary>
        /// Reads the image data and label associated with it.
        /// </summary>
        /// <returns></returns>
        public CharacterImage ReadNext() 
        {
            byte[,] img = new byte[m_imgWidth, m_imgHeight];
            int label = m_labelReader.ReadByte();

            // read img data
            for (int y = 0; y < m_imgHeight; y++) 
            {
                for (int x = 0; x < m_imgWidth; x++)
                {
                    byte b = m_imgReader.ReadByte();
                    img[x, y] = b;
                }
            }

            m_readIndex++;
            return new CharacterImage(img, Convert.ToChar("" + label));
        }

        /// <summary>
        /// Reads the rest of the data if anymore is available
        /// </summary>
        /// <returns></returns>
        public CharacterImage[] ReadRest()
        {
            CharacterImage[] data = new CharacterImage[m_numLabels - m_readIndex];

            // read all of the remaining data
            while (HasNext())
            {
                data[m_readIndex] = ReadNext();
            }

            return data;
        }

        /// <summary>
        /// True if there is more data to be read.
        /// </summary>
        /// <returns></returns>
        public bool HasNext()
        {
            return m_readIndex < m_numLabels;
        }

        /// <summary>
        /// Disposes IO resources that the MnistReader is using
        /// </summary>
        public void Dispose()
        {
            m_imgReader.Dispose();
            m_labelReader.Dispose();
        }
        
    }
}
