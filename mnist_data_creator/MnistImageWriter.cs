using System;
using System.Collections.Generic;
using System.Text;

using System.IO;

namespace mnist_data_creator
{
    public class MnistImageWriter
    {
        /// <summary>
        /// Path for the file
        /// </summary>
        private String m_path;

        /// <summary>
        /// writer for a output stream
        /// </summary>
        private BinaryWriter m_out;

        /// <summary>
        /// image width
        /// </summary>
        private uint m_width;

        /// <summary>
        /// image height 
        /// </summary>
        private uint m_height;

        /// <summary>
        /// count of images
        /// </summary>
        private uint m_count;

        /// <summary>
        /// output stream
        /// </summary>
        private FileStream m_stream;

        public MnistImageWriter(string path, uint count, uint width, uint height)
        {
            m_path = path;
            m_width = width;
            m_height = height;
            m_count = count;

            m_stream = new FileStream(m_path, FileMode.OpenOrCreate);
            m_out = new BinaryWriter(m_stream);

            WriteHeader();
        }

        /// <summary>
        /// Writes the given image to the file
        /// </summary>
        /// <param name="imgs"></param>
        public void WriteImage(byte[,] img)
        {
            // read img data
            for (int x = 0; x < m_width; x++)
            {
                for (int y = 0; y < m_height; y++)
                {
                    m_out.Write(img[x, y]);
                }
            }
        }

        /// <summary>
        /// Writes all of the given images to the file
        /// </summary>
        /// <param name="imgs"></param>
        public void WriteImages(byte[][,] imgs)
        {
            foreach (byte[,] i in imgs) 
            {
                WriteImage(i);
            }
        }

        /// <summary>
        /// Writes all of the given images to the file
        /// </summary>
        /// <param name="imgs"></param>
        public void WriteImages(List<byte[,]> imgs)
        {
            foreach (byte[,] i in imgs)
            {
                WriteImage(i);
            }
        }

        /// <summary>
        /// writes header to file
        /// </summary>
        private void WriteHeader()
        {
            // write garbage
            m_out.Write((int)0);

            // num img
            m_out.Write(SwapEndianness(m_count));

            // width
            m_out.Write(SwapEndianness(m_width));

            // height
            m_out.Write(SwapEndianness(m_height));
        }

        /// <summary>
        /// Releases/disposes IO resources
        /// </summary>
        public void Dispose()
        {
            m_stream.Dispose();
            m_out.Dispose();
        }

        /// <summary>
        /// Releases/disposes IO resources
        /// </summary>
        public void Close()
        {
            Dispose();
        }

        /// <summary>
        /// Swaps the endian format
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private uint SwapEndianness(uint x)
        {
            return ((x & 0x000000ff) << 24) +  // First byte
                   ((x & 0x0000ff00) << 8) +   // Second byte
                   ((x & 0x00ff0000) >> 8) +   // Third byte
                   ((x & 0xff000000) >> 24);   // Fourth byte
        }
    }
}
