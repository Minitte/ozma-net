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
        private int m_readIndex;

        /// <summary>
        /// width of each img in the data
        /// </summary>
        public int ImgWidth
        {
            get
            {
                return m_imgWidth;
            }
        }

        /// <summary>
        /// height of each img in the data
        /// </summary>
        public int ImgHeight
        {
            get
            {
                return m_imgHeight;
            }
        }

        /// <summary>
        /// number of labels, this should be equal to number of images
        /// </summary>
        public int NumLabels
        {
            get
            {
                return m_numLabels;
            }
        }

        /// <summary>
        /// numbers of images, this should be equal to number of labels
        /// </summary>
        public int NumImgs
        {
            get
            {
                return m_numImgs;
            }
        }

        /// <summary>
        /// Constructor for a mnist data reader
        /// </summary>
        /// <param name="labelPath">path to a mnist file containing the labels</param>
        /// <param name="imgPath">path to a mnist file containing the images</param>
        public MnistReader(String labelPath, String imgPath) 
        {
            m_labelReader = new BinaryReader(new FileStream(labelPath, FileMode.Open));
            m_imgReader = new BinaryReader(new FileStream(imgPath, FileMode.Open));

            m_readIndex = 0;

            ReadHeader();
        }

        /// <summary>
        /// Reads the header for data
        /// </summary>
        private void ReadHeader() 
        {
            // header for labels
            m_labelReader.ReadInt32(); // discard first
            m_numLabels = ReadBigInt32(m_labelReader);

            // header for imgs
            m_imgReader.ReadInt32(); // discard first
            m_numImgs = ReadBigInt32(m_imgReader);
            m_imgWidth = ReadBigInt32(m_imgReader);
            m_imgHeight = ReadBigInt32(m_imgReader);
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
            for (int x = 0; x < m_imgWidth; x++)
            {
                for (int y = 0; y < m_imgHeight; y++) 
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
            int i = 0;

            // read all of the remaining data
            while (HasNext())
            {
                data[i++] = ReadNext();
            }

            return data;
        }

        /// <summary>
        /// Reads all of the data from the start
        /// </summary>
        /// <returns></returns>
        public CharacterImage[] ReadAllFromStart()
        {
            m_labelReader.BaseStream.Seek(0, SeekOrigin.Begin);
            m_imgReader.BaseStream.Seek(0, SeekOrigin.Begin);

            ReadHeader(); // forwards the stream past the header
            m_readIndex = 0;

            return ReadRest();
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

        /// <summary>
        /// Reads the next 4 byte and converts to Big Endian format
        /// </summary>
        /// <param name="br"></param>
        /// <returns></returns>
        private int ReadBigInt32(BinaryReader br)
        {
            var bytes = br.ReadBytes(sizeof(Int32));
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }

    }
}
