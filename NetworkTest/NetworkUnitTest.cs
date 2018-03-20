using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using wpf_ozma_net.neural_network;

namespace NetworkTest
{
    [TestClass]
    public class NetworkUnitTest
    {
        [TestMethod]
        public void TestNetworkSetup()
        {
            int[] layerSettings = { 1, 4, 1 };
            Network network = new Network(layerSettings);
        }
    }
}
