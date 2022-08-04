using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Velentr.AbstractShapes;
using Velentr.Scaling;

using Point = Microsoft.Xna.Framework.Point;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace CoreDev
{
    [DebuggerDisplay("{_name}: {ScalingBox}")]
    public class Box
    {
        public Scalar ScalingBox;
        private Texture2D _texture;
        private Box _parentBox;
        private Color _color;
        private Rectangle _sourceRectangle = new Rectangle(0, 0, 1, 1);
        private string _name;

        public Box(string name, GraphicsDevice graphicsDevice, Rectangle bounds, Dimensions virtualBounds, Color color, Box parentBox = null)
        {
            _name = name;
            ScalingBox = parentBox == null
                ? new Scalar(bounds.X, bounds.Y, bounds.Width, bounds.Height, null, virtualBounds.Width, virtualBounds.Height)
                : new Scalar(bounds.X, bounds.Y, bounds.Width, bounds.Height, parentBox.ScalingBox, virtualBounds.Width, virtualBounds.Height);
            _color = color;
            _parentBox = parentBox;
            _texture = new Texture2D(graphicsDevice, 1, 1);
            _texture.SetData(new Color[] { Color.White });
        }

        public Point GetInternalCoordsForMouseCoords()
        {
            return this.ScalingBox.ConvertRootToVirtual(Mouse.GetState().X, Mouse.GetState().Y).ToXnaPoint();
        }

        public Point GetActualCoordsForInternalCoordsForMouseCoords()
        {
            return this.ScalingBox.ConvertVirtualToRoot(this.ScalingBox.ConvertRootToVirtual(Mouse.GetState().X, Mouse.GetState().Y)).ToXnaPoint();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var rectangle = this.ScalingBox.ConvertVirtualToRoot(0, 0, this.ScalingBox.VirtualDimensions.Width, this.ScalingBox.VirtualDimensions.Height);
            
            spriteBatch.Draw(_texture, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height), _sourceRectangle, _color);
        }
    }
}
