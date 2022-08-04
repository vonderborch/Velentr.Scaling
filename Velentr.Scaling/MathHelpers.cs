using System;

namespace Velentr.Scaling
{
    /// <summary>
    ///     The mathematics helpers.
    /// </summary>
    internal static class MathHelpers
    {
        /// <summary>
        ///     (Immutable) the tolerance.
        /// </summary>
        private static readonly double TOLERANCE = 0.0000001;

        /// <summary>
        ///     Tests if two double objects are considered equal.
        /// </summary>
        /// <param name="a">    Double to be compared. </param>
        /// <param name="b">    Double to be compared. </param>
        /// <returns>
        ///     True if the objects are considered equal, false if they are not.
        /// </returns>
        public static bool Equals(double a, double b)
        {
            return Math.Abs(a - b) < TOLERANCE;
        }
    }
}
