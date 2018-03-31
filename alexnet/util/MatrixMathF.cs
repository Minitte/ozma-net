using System;
using System.Collections.Generic;
using System.Text;

namespace alexnet.util
{
    public class MatrixMathF
    {

        private MatrixMathF() { }

        /// <summary>
        /// Multiplies two matrixs assuming input matrixs can be multipled and returns a new matrix with result
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static float[,] Multiply(float[,] first, float[,] second)
        {
            int resultRows = first.GetLength(0); // rows of first
            int resultCols = second.GetLength(1); // cols of second

            // sets correct result matrix size
            float[,] result = new float[resultRows, resultCols];

            // multiply
            for (int row = 0; row < resultRows; row++)
            {
                for (int col = 0; col < resultCols; col++)
                {

                    // row * col thing
                    for (int i = 0; i < resultCols; i++)
                    {
                        result[row, col] += first[row, i] * second[i, col];
                    }

                }
            }

            return result;
        }

        /// <summary>
        /// Multiplies many two matrixs with each other assuming input matrixs can be multipled and returns a new matrix with result
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static float[][,] Multiply(float[][,] first, float[][,] second)
        {
            float[][,] result = new float[second.Length][,];

            for (int i = 0; i < first.Length; i++)
            {
                result[i] = Multiply(first[i], second[i]);
            }

            return result;
        }

        /// <summary>
        /// Multiplies many two matrixs with each other assuming input matrixs can be multipled and returns a new matrix with result
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static float[][,] Multiply(float[][,] first, float[,] second)
        {
            float[][,] result = new float[second.Length][,];

            for (int i = 0; i < first.Length; i++)
            {
                result[i] = Multiply(first[i], second);
            }

            return result;
        }

        public static float[,] Add(float[,] first, float[,] second)
        {
            int resultRows = first.GetLength(0); // rows of first
            int resultCols = second.GetLength(1); // cols of second

            // sets correct result matrix size
            float[,] result = new float[resultRows, resultCols];

            // add
            for (int row = 0; row < resultRows; row++)
            {
                for (int col = 0; col < resultCols; col++)
                {
                    result[row, col] = first[row, col] + second[row, col];
                }
            }

            return result;
        }

        public static float[,] Minus(float[,] first, float[,] second)
        {
            int resultRows = first.GetLength(0); // rows of first
            int resultCols = second.GetLength(1); // cols of second

            // sets correct result matrix size
            float[,] result = new float[resultRows, resultCols];

            // add
            for (int row = 0; row < resultRows; row++)
            {
                for (int col = 0; col < resultCols; col++)
                {
                    result[row, col] = first[row, col] - second[row, col];
                }
            }

            return result;
        }

        public static float[,] Minus(float[,] first, float[] second)
        {
            int resultRows = first.GetLength(0); // rows of first
            int resultCols = second.GetLength(1); // cols of second

            // sets correct result matrix size
            float[,] result = new float[resultRows, resultCols];

            // add
            for (int row = 0; row < resultRows; row++)
            {
                for (int col = 0; col < resultCols; col++)
                {
                    result[row, col] = first[row, col] - second[col];
                }
            }

            return result;
        }

        /// <summary>
        /// randomizes all numbers in the array between 0.0 and 1.0
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static void RandomN(float[,] arr)
        {
            int resultRows = arr.GetLength(0); 
            int resultCols = arr.GetLength(1);
            Random rand = new Random();

            for (int row = 0; row < resultRows; row++)
            {
                for (int col = 0; col < resultCols; col++)
                {
                    arr[row, col] = (float)rand.NextDouble();
                }
            }
        }

        /// <summary>
        /// Python zip
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float[][][,] Zip(float[][,] a, float[][,] b)
        {
            float[][][,] result = new float[a.Length][][,];
            for (int i = 0; i < a.Length; i++)
            {
                result[i] = new float[2][,];
                result[i][0] = a[i];
                result[i][1] = b[i];
            }

            return result;
        }

        /// <summary>
        /// Converts 1d array into a "x by 1" matrix
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static float[,] ToMatrix(float[] arr)
        {
            float[,] result = new float[arr.Length, 1];
            for (int i = 0; i < arr.Length; i++)
            {
                result[i, 0] = arr[i];
            }
            return result;
        }

        /// <summary>
        /// Converts a matrix into a 1d array row by row
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static float[] ToArray(float[,] matrix)
        {
            int resultRows = matrix.GetLength(0);
            int resultCols = matrix.GetLength(1);

            float[] result = new float[resultRows * resultCols];
            int index = 0;

            for (int row = 0; row < resultRows; row++)
            {
                for (int col = 0; col < resultCols; col++)
                {
                    result[index++] = matrix[row, col];
                }
            }

            return result;
        }

        /// <summary>
        /// Applies sigmoid to all values of the matrix
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static float[,] Sigmoid(float[,] matrix)
        {
            int resultRows = matrix.GetLength(0); // rows of first
            int resultCols = matrix.GetLength(1); // cols of second

            // sets correct result matrix size
            float[,] result = new float[resultRows, resultCols];

            for (int row = 0; row < resultRows; row++)
            {
                for (int col = 0; col < resultCols; col++)
                {
                    result[row, col] = MathF.Sigmoid(matrix[row, col]);
                }
            }

            return result;
        }

        /// <summary>
        /// Inverse of applying sigmoid to all values of the matrix
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static float[,] SigmoidPrime(float[,] matrix)
        {
            int resultRows = matrix.GetLength(0); // rows of first
            int resultCols = matrix.GetLength(1); // cols of second

            // sets correct result matrix size
            float[,] result = new float[resultRows, resultCols];

            for (int row = 0; row < resultRows; row++)
            {
                for (int col = 0; col < resultCols; col++)
                {
                    result[row, col] = MathF.SigmoidPrimes(matrix[row, col]);
                }
            }

            return result;
        }

        /// <summary>
        /// Creates another matrix with the same row/col but zeroed out
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static float[,] Zeros(float[,] matrix)
        {
            return new float[matrix.GetLength(0), matrix.GetLength(1)];
        }

        /// <summary>
        /// Creates a list of matrixs with the same dimensions but zeroed out
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static float[][,] Zeros(float[][,] matrix)
        {
            float[][,] result = new float[matrix.Length][,];

            for (int i = 0; i < matrix.Length; i++)
            {
                result[i] = new float[matrix[i].GetLength(0), matrix[i].GetLength(1)];
            }

            return result;
        }

        /// <summary>
        /// Creates a list of list of matrixs
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static float[][][,] Zeros(float[][,] matrix, int count)
        {
            float[][][,] result = new float[count][][,];

            for (int i = 0; i < count; i++)
            {
                result[i] = Zeros(matrix);
            }

            return result;
        }

        public static float[,] Transpose(float[,] matrix)
        {
            int mRows = matrix.GetLength(0); // rows of matrix
            int mCols = matrix.GetLength(1); // cols of matrix

            float[,] result = new float[mCols, mRows];

            for (int row = 0; row < mRows; row++)
            {
                for (int col = 0; col < mCols; col++)
                {
                    result[row, col] = matrix[col, row];
                }
            }

            return result;
        }

        public static float[][,] Transpose(float[][,] matrix)
        {
            float[][,] result = new float[matrix.Length][,];

            for (int i = 0; i < matrix.Length; i++)
            {
                result[i] = Transpose(matrix[i]);
            }

            return result;
        }
    }
}
