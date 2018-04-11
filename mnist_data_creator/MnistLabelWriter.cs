using System;
using System.Collections.Generic;
using System.Text;

using System.IO;

namespace mnist_data_creator
{
    public class MnistLabelWriter
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
        /// number of things
        /// </summary>
        private uint m_count;

        /// <summary>
        /// output stream
        /// </summary>
        private FileStream m_stream;

        public MnistLabelWriter(string path, uint count)
        {
            m_path = path;
            m_count = count;

            m_stream = new FileStream(m_path, FileMode.OpenOrCreate);
            m_out = new BinaryWriter(m_stream);

            WriteHeader();
        }

        /// <summary>
        /// Writes the given label to the file
        /// </summary>
        /// <param name="label"></param>
        public void WriteLabel(byte label)
        {
            m_out.Write(label);
        }

        /// <summary>
        /// Writes all of the given labels to the file
        /// </summary>
        /// <param name="Labels"></param>
        public void WriteLabels(byte[] Labels)
        {
            foreach (byte l in Labels) 
            {
                WriteLabel(l);
            }
        }

        /// <summary>
        /// Writes all of the given Labels to the file
        /// </summary>
        /// <param name="Labels"></param>
        public void WriteLabels(List<byte> Labels)
        {
            foreach (byte l in Labels)
            {
                WriteLabel(l);
            }
        }

        /// <summary>
        /// writes header to file
        /// </summary>
        private void WriteHeader()
        {
            // write garbage
            m_out.Write((int)0);

            // num labels
            m_out.Write(SwapEndianness(m_count));
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
