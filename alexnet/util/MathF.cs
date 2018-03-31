using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.CompilerServices;

namespace alexnet.util
{
    /// <summary>
    /// A collection of math functions that uses floats instead of the standard doubles from c# library.
    /// </summary>
    public class MathF
    {

        public static float E = (float)Math.E;

        private MathF() { }

        /// <summary>
        /// Raises e raised to the power of value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Exp(float value)
        {
            return (float)Math.Pow(Math.E, value);
        }

        /// <summary>
        /// Sigmoid function
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sigmoid(float value)
        {
            return 1f / (1f + Exp(-value));
        }

        /// <summary>
        /// Sigmoid prime function
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SigmoidPrimes(float value)
        {
            return Sigmoid(value) * (1f - Sigmoid(value));
        }


    }
}
