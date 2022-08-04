using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Velentr.AbstractShapes;
using Velentr.Debugging;
using Velentr.Font;
using Velentr.Scaling;

using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace CoreDev
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private FpsTracker _frameCounter = new FpsTracker(10);
        private PerformanceTracker _performance = new PerformanceTracker(10, enableFpsTracker: true);
        private string _baseTitle = "Velentr.ScalingAndResolution.DevEnv";
        private string _decimals = "0.000";

        // Font stuff for status updates
        private string _fontName = "MontserratRegular-RpK6l.otf";
        private FontManager _fontManager;
        private Font _font;

        // Stuff we're testing
        private List<Box> boxes = new List<Box>();

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
            _baseTitle = $"{_baseTitle} | FPS: {{0:{_decimals}}} | TPS: {{1:{_decimals}}} | CPU: {{2:{_decimals}}}% | Memory: {{3:{_decimals}}} MB";
            _fontManager = new FontManager(GraphicsDevice);
            _font = _fontManager.GetFont(_fontName, 12);

            var parent = new Box("p", GraphicsDevice, new Rectangle(128, 128, 512, 512), new Dimensions(1024, 1024), Color.Black);
            var child = new Box("c1", GraphicsDevice, new Rectangle(32, 32, 512, 512), new Dimensions(1024, 1024), Color.Red, parent);
            var child2 = new Box("c2", GraphicsDevice, new Rectangle(0, 0, 512, 512), new Dimensions(1024, 1024), Color.Blue, child);

            this.boxes = new List<Box>() {parent, child, child2};
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            _performance.Update(gameTime.ElapsedGameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _frameCounter.Update(gameTime.ElapsedGameTime);
            Window.Title = string.Format(_baseTitle, _frameCounter.AverageFramesPerSecond, _performance.FpsTracker.AverageFramesPerSecond, _performance.CpuTracker.CpuPercent, _performance.MemoryTracker.MemoryUsageMb);

            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();


            for (int i = 0; i < this.boxes.Count; i++)
            {
                this.boxes[i].Draw(this._spriteBatch);
            }

            var y = 0;
            this._spriteBatch.DrawString(this._font, $"(Actual Mouse Coords: {Mouse.GetState().X}, {Mouse.GetState().Y})", new Vector2(16, 0), Color.Black);
            for (int i = 0; i < this.boxes.Count; i++)
            {
                y += 14;
                this._spriteBatch.DrawString(this._font, $"Box {i} D: {this.boxes[i].ScalingBox}, RMC: {this.boxes[i].GetInternalCoordsForMouseCoords()}, AMC: {this.boxes[i].GetActualCoordsForInternalCoordsForMouseCoords()}", new Vector2(16, y), Color.Black);
            }

            _spriteBatch.End();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
