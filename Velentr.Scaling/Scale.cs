using System.Diagnostics;

using Velentr.AbstractShapes;

namespace Velentr.Scaling
{
    /// <summary>
    ///     A scale.
    /// </summary>
    [DebuggerDisplay("{ToString()}")]
    public struct Scale
    {
        /// <summary>
        ///     The X coordinate.
        /// </summary>
        public double X;

        /// <summary>
        ///     The Y coordinate.
        /// </summary>
        public double Y;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="x">    The x coordinate. </param>
        /// <param name="y">    The y coordinate. </param>
        public Scale(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="parentWidth">  Width of the parent. </param>
        /// <param name="childWidth">   Width of the child. </param>
        /// <param name="parentHeight"> Height of the parent. </param>
        /// <param name="childHeight">  Height of the child. </param>
        public Scale(int parentWidth, int childWidth, int parentHeight, int childHeight)
        {
            this.X = parentWidth / (double) childWidth;
            this.Y = parentHeight / (double) childHeight;
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="child">    The child. </param>
        /// <param name="parent">   The parent. </param>
        public Scale(Dimensions child, Dimensions parent)
        {
            this.X = parent.Width / (double) child.Width;
            this.Y = parent.Height / (double) child.Height;
        }

        /// <summary>
        ///     Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>
        ///     The fully qualified type name.
        /// </returns>
        /// <seealso cref="System.ValueType.ToString()" />
        public override string ToString()
        {
            return $"({this.X}x, {this.Y}x)";
        }

        /// <summary>
        ///     Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        ///     A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        /// <seealso cref="System.ValueType.GetHashCode()" />
        public override int GetHashCode()
        {
            return this.X.GetHashCode() ^ this.Y.GetHashCode();
        }

        /// <summary>
        ///     Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">  The object to compare with the current instance. </param>
        /// <returns>
        ///     true if <paramref name="obj">obj</paramref> and this instance are the same type and represent
        ///     the same value; otherwise, false.
        /// </returns>
        /// <seealso cref="System.ValueType.Equals(object)" />
        public override bool Equals(object obj)
        {
            if (!(obj is Scale) || obj == null)
            {
                return false;
            }

            var p = (Scale) obj;

            return MathHelpers.Equals(this.X, p.X) && MathHelpers.Equals(this.Y, p.Y);
        }
    }
}
