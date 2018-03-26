using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ozmanet.util;
using ozmanet.neural_network;

namespace NetworkTest
{
    [TestClass]
    public class MnistReaderUnitTest
    {

        private static MnistReader reader;

        [TestInitialize()]
        public void Initialize()
        {
            reader = new MnistReader(
                    "../../../data/digits/test/t10k-labels.idx1-ubyte",     // path for labels
                    "../../../data/digits/test/t10k-images.idx3-ubyte");    // path for imgs
        }

        [TestCleanup()]
        public void Cleanup()
        {
            reader.Dispose();
        }

        [TestMethod]
        public void TestWidthHeight()
        {
            Assert.AreEqual(28, reader.ImgWidth);
            Assert.AreEqual(28, reader.ImgHeight);
        }

        [TestMethod]
        public void TestNumData()
        {
            Assert.AreEqual(10000, reader.NumImgs);
            Assert.AreEqual(10000, reader.NumLabels);
        }

        [TestMethod]
        public void TestReadRest()
        {
            int initalRead = 14;

            for (int i = 0; i < initalRead; i++)
            {
                reader.ReadNext();
            }

            CharacterImage[] data = reader.ReadRest();
            Assert.AreEqual(data.Length, 10000 - initalRead);
        }
    }
}
