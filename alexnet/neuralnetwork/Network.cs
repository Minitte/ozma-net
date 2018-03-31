using System;
using System.Collections.Generic;
using System.Text;
using alexnet.util;
using System.Linq;

namespace alexnet.neuralnetwork
{
    public class Network
    {

        // index constant for better readability with zipped biases / weight 
        readonly int BIASES_INDEX = 0; // biases
        readonly int WEIGHT_INDEX = 1; // weight

        private int m_numLayers;
        private int[] m_layers;
        private float[][,] m_biases;
        private float[][,] m_weights;

        public Network(NetworkSettings settings)
        {
            m_numLayers = m_layers.Length;
            m_layers = settings.ToArray();

            // randomize matrix for biases
            m_biases = new float[m_numLayers - 1][,];
            for (int i = 1; 1 < m_numLayers; i++)
            {
                m_biases[i - 1] = new float[m_layers[i], 1];
                MatrixMathF.RandomN(m_biases[i - 1]);
            }

            // randomize matrix for weights
            m_weights = new float[m_numLayers - 1][,];
            for (int i = 1; 1 < m_numLayers; i++)
            {
                m_weights[i - 1] = new float[m_layers[i - 1], m_layers[i]];
            }
        }

        public float[][,] FeedFoward(float[] input)
        {
            float[][][,] zipped = MatrixMathF.Zip(m_biases, m_weights);
            float[][,] result = new float[zipped.Length][,];
            float[,] inputMatrix = MatrixMathF.ToMatrix(input);

            for (int i = 0; i < zipped.Length; i++)
            {
                float[,] mult = MatrixMathF.Multiply(zipped[i][WEIGHT_INDEX], inputMatrix);
                float[,] add = MatrixMathF.Add(mult, zipped[i][BIASES_INDEX]);
                result[i] = MatrixMathF.Sigmoid(add);
            }

            return result;
        }

        public void Train(float[][,] input, float[][] expected, float learningRate)
        {
            float[][][,] nabla_b = MatrixMathF.Zeros(m_biases, m_biases.Length);
            float[][][,] nabla_w = MatrixMathF.Zeros(m_weights, m_weights.Length);

            for (int i = 0; i < input.Length; i++)
            {
                BackProp(input[i], expected[i]);
            }

        }

        public void BackProp(float[,] input, float[] expected)
        {
            float[][][,] nabla_b = MatrixMathF.Zeros(m_biases, m_biases.Length);
            float[][][,] nabla_w = MatrixMathF.Zeros(m_weights, m_weights.Length);

            // feed foward
            float[,] activation = input;

            // list to store all activations, layer by layer
            List<float[,]> activations = new List<float[,]>();
            activations.Add(input);

            // list to store all z vectors layer by layer
            List<float[,]> zs = new List<float[,]>();

            float[][][,] zipped = MatrixMathF.Zip(m_biases, m_weights);

            for (int i = 0; i < zipped.Length; i++)
            {
                float[,] z = MatrixMathF.Multiply(zipped[i][WEIGHT_INDEX], activation);
                z = MatrixMathF.Add(z, zipped[i][BIASES_INDEX]);
                zs.Add(z);
                activation = MatrixMathF.Sigmoid(z);
                activations.Add(activation);
            }

            // backward pass
            float[,] delta = CostDrivative(activations[activations.Count() - 2], expected);
            delta = MatrixMathF.Multiply(delta, MatrixMathF.SigmoidPrime(zs[zs.Count() - 2]));

            nabla_b[nabla_b.Length - 2] = delta;

            nabla_w[nabla_w.Length - 3] = MatrixMathF.Multiply(delta, MatrixMathF.Transpose(activations[activations.Count() - 3]));

            // l represents the layer
            for (int l = 2; l < m_numLayers; l++)
            {
                float[,] z = zs[zs.Count() - 1 - l];
                float[,] sp = MatrixMathF.SigmoidPrime(z);
                float[,] weightTrans = MatrixMathF.Transpose(m_weights[m_weights.Length - l]);

                float[,] delta2 = MatrixMathF.Multiply(weightTrans, delta);

                //nabla_b[nabla_b.Length - 1 - l] = delta;
            }
        }

        private float[,] CostDrivative(float[,] outActivation, float[] expected)
        {
            return MatrixMathF.Minus(outActivation, expected);
        }

    }
}
