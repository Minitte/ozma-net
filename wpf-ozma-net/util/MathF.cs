using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.CompilerServices;

namespace ozmanet.util
{
    /// <summary>
    /// A collection of math functions that uses floats instead of the standard doubles from c# library.
    /// </summary>
    public class MathF
    {

        public static double E = Math.E;

        private MathF() { }

        /// <summary>
        /// Raises e raised to the power of value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Exp(double value)
        {
            return Math.Pow(Math.E, value);
        }

        /// <summary>
        /// Sigmoid function
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Sigmoid(double value)
        {
            return 1.0 / (1.0 + Exp(-value));
        }

        /// <summary>
        /// Sigmoid prime function
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double SigmoidPrimes(double value)
        {
            return Sigmoid(value) * (1.0 - Sigmoid(value));
        }


    }
}
