using System;

namespace squigglenet.neuralnetwork
{
    public class Dendrite
    {
        public double Weight { get; set; }

        public Dendrite()
        {
            //CryptoRandom n = new CryptoRandom();
            this.Weight = new Random().NextDouble();
        }
    }

}
