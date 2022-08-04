using System;
using System.Diagnostics;

using Velentr.AbstractShapes;

namespace Velentr.Scaling
{
    /// <summary>
    ///     A scalar.
    /// </summary>
    [DebuggerDisplay("{ToString()}")]
    public class Scalar
    {
        /// <summary>
        ///     (Immutable) the default scale.
        /// </summary>
        private const double DefaultScale = 1d;

        /// <summary>
        ///     The bounds.
        /// </summary>
        private Rectangle? _bounds;

        /// <summary>
        ///     The coordinates.
        /// </summary>
        private Point _coordinates;

        /// <summary>
        ///     The dimensions.
        /// </summary>
        private Dimensions _dimensions;

        /// <summary>
        ///     The virtual dimensions.
        /// </summary>
        private Dimensions _virtualDimensions;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <exception cref="ArgumentException">
        ///     Thrown when one or more arguments have unsupported or
        ///     illegal values.
        /// </exception>
        /// <param name="x">                The x coordinate. </param>
        /// <param name="y">                The y coordinate. </param>
        /// <param name="width">            The width. </param>
        /// <param name="height">           The height. </param>
        /// <param name="parentScalar">
        ///     (Optional)
        ///     The parent scalar.
        /// </param>
        /// <param name="virtualWidth">     (Optional) Width of the virtual. </param>
        /// <param name="virtualHeight">    (Optional) Height of the virtual. </param>
        public Scalar(
            int x
          , int y
          , int width
          , int height
          , Scalar parentScalar = null
          , int? virtualWidth = null
          , int? virtualHeight = null
        )
        {
            if ((virtualWidth != null && virtualHeight == null) || (virtualWidth == null && virtualHeight != null))
            {
                throw new ArgumentException($"{nameof(virtualWidth)} and {nameof(virtualHeight)} must both be populated if one is populated!");
            }

            this.Bounds = new Rectangle(x, y, width, height);
            this.ParentScalar = parentScalar;

            if (virtualWidth == null && parentScalar != null)
            {
                this.Bounds = new Rectangle(x, y, parentScalar.Bounds.Width, parentScalar.Bounds.Height);
                this.VirtualDimensions = new Dimensions(width, height);
            }
            else
            {
                this.VirtualDimensions = virtualWidth == null ? new Dimensions(width, height) : new Dimensions((int) virtualWidth, (int) virtualHeight);
            }

            CalculateProperties();
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="position">             The position. </param>
        /// <param name="dimensions">           The dimensions. </param>
        /// <param name="parentScalar">
        ///     (Optional)
        ///     The parent scalar.
        /// </param>
        /// <param name="virtualDimensions">    (Optional) The virtual dimensions. </param>
        public Scalar(Point position, Dimensions dimensions, Scalar parentScalar = null, Dimensions? virtualDimensions = null)
        {
            this.Bounds = new Rectangle(position.IntX, position.IntY, dimensions.Width, dimensions.Height);
            this.ParentScalar = parentScalar;

            if (virtualDimensions == null && parentScalar != null)
            {
                this.Bounds = new Rectangle(position.IntX, position.IntY, parentScalar.Bounds.Width, parentScalar.Bounds.Height);
                this.VirtualDimensions = dimensions;
            }
            else
            {
                this.VirtualDimensions = virtualDimensions ?? new Dimensions(dimensions.Width, dimensions.Height);
            }

            CalculateProperties();
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="bounds">               The bounds. </param>
        /// <param name="parentScalar">
        ///     (Optional)
        ///     The parent scalar.
        /// </param>
        /// <param name="virtualDimensions">    (Optional) The virtual dimensions. </param>
        public Scalar(Rectangle bounds, Scalar parentScalar = null, Dimensions? virtualDimensions = null)
        {
            this.Bounds = bounds;
            this.ParentScalar = parentScalar;
            if (virtualDimensions == null && parentScalar != null)
            {
                this.Bounds = new Rectangle(bounds.X, bounds.Y, parentScalar.Bounds.Width, parentScalar.Bounds.Height);
                this.VirtualDimensions = new Dimensions(bounds.Width, bounds.Height);
            }
            else
            {
                this.VirtualDimensions = virtualDimensions ?? new Dimensions(bounds.Width, bounds.Height);
            }

            CalculateProperties();
        }

        /// <summary>
        ///     Gets or sets the bounds.
        /// </summary>
        /// <value>
        ///     The bounds.
        /// </value>
        public Rectangle Bounds
        {
            get
            {
                if (this._bounds == null)
                {
                    this._bounds = new Rectangle(this.Coordinates.IntX, this.Coordinates.IntY, this.Dimensions.Width, this.Dimensions.Height);
                }

                return (Rectangle) this._bounds;
            }

            set
            {
                this._coordinates.X = value.X;
                this._coordinates.Y = value.Y;
                this._dimensions.Width = value.Width;
                this._dimensions.Height = value.Height;
                CalculateProperties();
            }
        }

        /// <summary>
        ///     Gets or sets the coordinates.
        /// </summary>
        /// <value>
        ///     The coordinates.
        /// </value>
        public Point Coordinates
        {
            get => this._coordinates;

            set
            {
                this._coordinates.X = value.X;
                this._coordinates.Y = value.Y;
                CalculateProperties();
            }
        }

        /// <summary>
        ///     Gets or sets the dimensions.
        /// </summary>
        /// <value>
        ///     The dimensions.
        /// </value>
        public Dimensions Dimensions
        {
            get => this._dimensions;

            set
            {
                this._dimensions.Width = value.Width;
                this._dimensions.Height = value.Height;
                CalculateProperties();
            }
        }

        /// <summary>
        ///     Gets or sets the internal scale.
        /// </summary>
        /// <value>
        ///     The internal scale.
        /// </value>
        public Scale InternalScale { get; private set; }

        /// <summary>
        ///     Gets or sets the parent scalar.
        /// </summary>
        /// <value>
        ///     The parent scalar.
        /// </value>
        public Scalar ParentScalar { get; set; }

        /// <summary>
        ///     Gets or sets the virtual dimensions.
        /// </summary>
        /// <value>
        ///     The virtual dimensions.
        /// </value>
        public Dimensions VirtualDimensions
        {
            get => this._virtualDimensions;

            set
            {
                this._virtualDimensions.Width = value.Width;
                this._virtualDimensions.Height = value.Height;
                CalculateProperties();
            }
        }

        /// <summary>
        ///     Convert root to virtual.
        /// </summary>
        /// <param name="point">                        The point. </param>
        /// <param name="alreadyScaledToParentScale">
        ///     (Optional) True to already scaled to parent
        ///     scale.
        /// </param>
        /// <returns>
        ///     The root converted to virtual.
        /// </returns>
        public Point ConvertRootToVirtual(Point point, bool alreadyScaledToParentScale = false)
        {
            if (!alreadyScaledToParentScale && this.ParentScalar != null)
            {
                point = this.ParentScalar.ConvertRootToVirtual(point);
            }

            point.X = IsDefaultScale(this.InternalScale.X) ? point.X - this.Coordinates.X : point.X * this.InternalScale.X - this.Coordinates.X * this.InternalScale.X;
            point.Y = IsDefaultScale(this.InternalScale.Y) ? point.Y - this.Coordinates.Y : point.Y * this.InternalScale.Y - this.Coordinates.Y * this.InternalScale.Y;

            return point;
        }

        /// <summary>
        ///     Convert root to virtual.
        /// </summary>
        /// <param name="x">                        The x coordinate. </param>
        /// <param name="y">                        The y coordinate. </param>
        /// <param name="alreadyScaledInParent">    (Optional) True to already scaled in parent. </param>
        /// <returns>
        ///     The root converted to virtual.
        /// </returns>
        public Point ConvertRootToVirtual(double x, double y, bool alreadyScaledInParent = false)
        {
            return ConvertRootToVirtual(new Point(x, y), alreadyScaledInParent);
        }

        /// <summary>
        ///     Convert root to virtual.
        /// </summary>
        /// <param name="x">                        The x coordinate. </param>
        /// <param name="y">                        The y coordinate. </param>
        /// <param name="alreadyScaledInParent">    (Optional) True to already scaled in parent. </param>
        /// <returns>
        ///     The root converted to virtual.
        /// </returns>
        public Point ConvertRootToVirtual(int x, int y, bool alreadyScaledInParent = false)
        {
            return ConvertRootToVirtual(new Point(x, y), alreadyScaledInParent);
        }

        /// <summary>
        ///     Convert virtual to root.
        /// </summary>
        /// <param name="point">    The point. </param>
        /// <returns>
        ///     The virtual converted to root.
        /// </returns>
        public Point ConvertVirtualToRoot(Point point)
        {
            point.X = IsDefaultScale(this.InternalScale.X) ? point.X + this.Coordinates.X : this.Coordinates.X + point.X / this.InternalScale.X;
            point.Y = IsDefaultScale(this.InternalScale.Y) ? point.Y + this.Coordinates.Y : this.Coordinates.Y + point.Y / this.InternalScale.Y;

            return this.ParentScalar?.ConvertVirtualToRoot(point) ?? point;
        }

        /// <summary>
        ///     Convert virtual to root.
        /// </summary>
        /// <param name="x">    The x coordinate. </param>
        /// <param name="y">    The y coordinate. </param>
        /// <returns>
        ///     The virtual converted to root.
        /// </returns>
        public Point ConvertVirtualToRoot(double x, double y)
        {
            return ConvertVirtualToRoot(new Point(x, y));
        }

        /// <summary>
        ///     Convert virtual to root.
        /// </summary>
        /// <param name="x">    The x coordinate. </param>
        /// <param name="y">    The y coordinate. </param>
        /// <returns>
        ///     The virtual converted to root.
        /// </returns>
        public Point ConvertVirtualToRoot(int x, int y)
        {
            return ConvertVirtualToRoot(new Point(x, y));
        }

        /// <summary>
        ///     Convert virtual to root.
        /// </summary>
        /// <param name="rectangle">    The rectangle. </param>
        /// <returns>
        ///     The virtual converted to root.
        /// </returns>
        public Rectangle ConvertVirtualToRoot(Rectangle rectangle)
        {
            var xy = ConvertVirtualToRoot(new Point(rectangle.X, rectangle.Y));
            var wh = ConvertVirtualToRoot(new Point(rectangle.Width, rectangle.Height)) - xy;

            return new Rectangle(xy.X, xy.Y, wh.X, wh.Y);
        }

        /// <summary>
        ///     Convert virtual to root.
        /// </summary>
        /// <param name="rectangle">    The rectangle. </param>
        /// <returns>
        ///     The virtual converted to root.
        /// </returns>
        public RectangleD ConvertVirtualToRoot(RectangleD rectangle)
        {
            var xy = ConvertVirtualToRoot(new Point(rectangle.X, rectangle.Y));
            var wh = ConvertVirtualToRoot(new Point(rectangle.Width, rectangle.Height)) - xy;

            return new RectangleD(xy.X, xy.Y, wh.X, wh.Y);
        }

        /// <summary>
        ///     Convert virtual to root.
        /// </summary>
        /// <param name="x">        The x coordinate. </param>
        /// <param name="y">        The y coordinate. </param>
        /// <param name="width">    The width. </param>
        /// <param name="height">   The height. </param>
        /// <returns>
        ///     The virtual converted to root.
        /// </returns>
        public RectangleD ConvertVirtualToRoot(double x, double y, double width, double height)
        {
            return ConvertVirtualToRoot(new RectangleD(x, y, width, height));
        }

        /// <summary>
        ///     Convert virtual to root.
        /// </summary>
        /// <param name="x">        The x coordinate. </param>
        /// <param name="y">        The y coordinate. </param>
        /// <param name="width">    The width. </param>
        /// <param name="height">   The height. </param>
        /// <returns>
        ///     The virtual converted to root.
        /// </returns>
        public Rectangle ConvertVirtualToRoot(int x, int y, int width, int height)
        {
            return ConvertVirtualToRoot(new Rectangle(x, y, width, height));
        }

        /// <summary>
        ///     Creates child scalar.
        /// </summary>
        /// <param name="bounds">               The bounds. </param>
        /// <param name="virtualDimensions">    (Optional) The virtual dimensions. </param>
        /// <returns>
        ///     The new child scalar.
        /// </returns>
        public Scalar CreateChildScalar(Rectangle bounds, Dimensions? virtualDimensions = null)
        {
            return new Scalar(bounds, this, virtualDimensions);
        }

        /// <summary>
        ///     Creates child scalar.
        /// </summary>
        /// <param name="position">             The position. </param>
        /// <param name="dimensions">           The dimensions. </param>
        /// <param name="virtualDimensions">    (Optional) The virtual dimensions. </param>
        /// <returns>
        ///     The new child scalar.
        /// </returns>
        public Scalar CreateChildScalar(Point position, Dimensions dimensions, Dimensions? virtualDimensions = null)
        {
            return new Scalar(position, dimensions, this, virtualDimensions);
        }

        /// <summary>
        ///     Creates child scalar.
        /// </summary>
        /// <param name="x">                The x coordinate. </param>
        /// <param name="y">                The y coordinate. </param>
        /// <param name="width">            The width. </param>
        /// <param name="height">           The height. </param>
        /// <param name="virtualWidth">     (Optional) Width of the virtual. </param>
        /// <param name="virtualHeight">    (Optional) Height of the virtual. </param>
        /// <returns>
        ///     The new child scalar.
        /// </returns>
        public Scalar CreateChildScalar(
            int x
          , int y
          , int width
          , int height
          , int? virtualWidth = null
          , int? virtualHeight = null
        )
        {
            return new Scalar(
                              x
                            , y
                            , width
                            , height
                            , this
                            , virtualWidth
                            , virtualHeight
                             );
        }

        /// <summary>
        ///     Gets root point from virtual percentage.
        /// </summary>
        /// <param name="horizontalPercentage"> (Optional) The horizontal percentage. Defaults to 0%. </param>
        /// <param name="horizontalSide">
        ///     (Optional) The horizontal side. Defaults to Side.Left,
        ///     must be either Side.Left or Side.Right.
        /// </param>
        /// <param name="verticalPercentage">   (Optional) The vertical percentage. Defaults to 0%. </param>
        /// <param name="verticalSide">
        ///     (Optional) The vertical side. Defaults to Side.Top, must
        ///     be either Side.Top or Side.Bottom.
        /// </param>
        /// <returns>
        ///     The root point from virtual percentage.
        /// </returns>
        public Point GetRootPointFromVirtualPercentage(double horizontalPercentage = 0d, Side horizontalSide = Side.Left, double verticalPercentage = 0d, Side verticalSide = Side.Top)
        {
            return ConvertVirtualToRoot(GetVirtualPointFromPercentage(horizontalPercentage, horizontalSide, verticalPercentage, verticalSide));
        }

        /// <summary>
        ///     Gets root point from virtual percentage.
        /// </summary>
        /// <param name="horizontalPercentage"> (Optional) The horizontal percentage. Defaults to 0%. </param>
        /// <param name="horizontalSide">
        ///     (Optional) The horizontal side. Defaults to Side.Left,
        ///     must be either Side.Left or Side.Right.
        /// </param>
        /// <param name="verticalPercentage">   (Optional) The vertical percentage. Defaults to 0%. </param>
        /// <param name="verticalSide">
        ///     (Optional) The vertical side. Defaults to Side.Top, must
        ///     be either Side.Top or Side.Bottom.
        /// </param>
        /// <returns>
        ///     The root point from virtual percentage.
        /// </returns>
        public Point GetVirtualPointFromPercentage(double horizontalPercentage = 0d, Side horizontalSide = Side.Left, double verticalPercentage = 0d, Side verticalSide = Side.Top)
        {
            if (horizontalSide == Side.Top || horizontalSide == Side.Bottom)
            {
                throw new ArgumentOutOfRangeException(nameof(horizontalSide), "Must be configured to either Side.Left or Side.Right!");
            }

            if (verticalSide == Side.Left || verticalSide == Side.Right)
            {
                throw new ArgumentOutOfRangeException(nameof(verticalSide), "Must be configured to either Side.Top or Side.Bottom!");
            }

            var point = new Point(0, 0);

            double percentage;

            // Find the X Coordinate if we are requested to...
            if (MathHelpers.Equals(horizontalPercentage, 0d))
            {
                percentage = horizontalPercentage;
                if (horizontalSide == Side.Right)
                {
                    percentage = 1 - percentage;
                }

                point.X = this.Coordinates.X + percentage * this.VirtualDimensions.Width;
            }

            // Find the Y Coordinate if we are requested to...
            if (MathHelpers.Equals(verticalPercentage, 0d))
            {
                percentage = verticalPercentage;
                if (verticalSide == Side.Bottom)
                {
                    percentage = 1 - percentage;
                }

                point.Y = this.Coordinates.Y + percentage * this.VirtualDimensions.Height;
            }

            return point;
        }

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        ///     A string that represents the current object.
        /// </returns>
        /// <seealso cref="System.Object.ToString()" />
        public override string ToString()
        {
            return $"x: {this.Bounds.X}, y:{this.Bounds.Y}, w:{this.Bounds.Width}, h:{this.Bounds.Height}, vd: {this.VirtualDimensions}, Has Parent? {this.ParentScalar != null}";
        }

        /// <summary>
        ///     Calculates the properties on a dimension change.
        /// </summary>
        protected virtual void CalculateProperties()
        {
            this.InternalScale = new Scale(this._dimensions, this._virtualDimensions);
            if (this._bounds != null)
            {
                this._bounds = new Rectangle(this.Coordinates.IntX, this.Coordinates.IntY, this.Dimensions.Width, this.Dimensions.Height);
            }
        }

        /// <summary>
        ///     Query if 'value' is default scale.
        /// </summary>
        /// <param name="value">    The value. </param>
        /// <returns>
        ///     True if default scale, false if not.
        /// </returns>
        private bool IsDefaultScale(double value)
        {
            return MathHelpers.Equals(DefaultScale, value);
        }
    }
}
