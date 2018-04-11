using System;
using System.Collections.Generic;
using System.Text;

using System.IO;

namespace ozmanet.util
{
    class MnistImageWriter
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
        /// list of image data
        /// </summary>
        private List<byte[,]> m_data;

        /// <summary>
        /// image width
        /// </summary>
        private uint m_width;

        /// <summary>
        /// image height 
        /// </summary>
        private uint m_height;

        /// <summary>
        /// output stream
        /// </summary>
        private FileStream m_stream;

        public MnistImageWriter(string path, uint width, uint height)
        {
            m_path = path;
            m_width = width;
            m_height = height;

            m_stream = new FileStream(m_path, FileMode.OpenOrCreate);
            m_out = new BinaryWriter(m_stream);
            m_data = new List<byte[,]>();

            WriteHeader();
        }

        /// <summary>
        /// Writes the given image to the file
        /// </summary>
        /// <param name="imgs"></param>
        public void WriteImage(byte[,] img)
        {
            m_data.Add(img);
        }

        /// <summary>
        /// Writes all of the given images to the file
        /// </summary>
        /// <param name="imgs"></param>
        public void WriteImages(byte[][,] imgs)
        {
            foreach (byte[,] i in imgs) 
            {
                m_data.Add(i);
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
                m_data.Add(i);
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
