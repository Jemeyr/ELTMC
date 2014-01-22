using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
     
    namespace CustomModelEffect
    {
        public class FrameRateCounter : DrawableGameComponent
        {
            public Vector2 Position { get; set; }
            public Vector2 Position2 { get; set; }

            public Color Color { get; set; }
            public Color Color2 { get; set; }

            private Color Color3;

            private ContentManager content;
            private SpriteBatch spriteBatch;
            private SpriteFont spriteFont;
     
            private int frameRate = 0;
            private int frameCounter = 0;
            private TimeSpan elapsedTime = TimeSpan.Zero;
     
            public FrameRateCounter(Game game, Vector2 position, Color color, Color color2)
                : base(game)
            {
                content = game.Content;
                Position = position;
                Position2 = new Vector2(Position.X + 1, Position.Y + 1);
                Color = color;
                Color2 = color2;
                Color3 = Color.Black;
            }
     
            protected override void LoadContent()
            {
                spriteBatch = new SpriteBatch(GraphicsDevice);
                spriteFont = content.Load<SpriteFont>("Font");
            }
     
            protected override void UnloadContent()
            {
                content.Unload();
            }
     
            public override void Update(GameTime gameTime)
            {
                elapsedTime += gameTime.ElapsedGameTime;
     
                if (elapsedTime > TimeSpan.FromSeconds(1))
                {
                    elapsedTime -= TimeSpan.FromSeconds(1);
                    frameRate = frameCounter;
                    frameCounter = 0;
                }
            }
     
            public override void Draw(GameTime gameTime)
            {
                frameCounter++;

                string text =   "CLUBS DAY VERSION\nFPS: " + frameRate;
                string bgtext = "NNNNNNNNNNNNNNNNNNNNNN\nNNNNNNNNNN";
                spriteBatch.Begin();
                spriteBatch.DrawString(spriteFont, bgtext, new Vector2(Position.X, Position.Y), Color3);
                spriteBatch.DrawString(spriteFont, bgtext, new Vector2(Position.X+2, Position.Y), Color3);
                spriteBatch.DrawString(spriteFont, bgtext, new Vector2(Position.X, Position.Y+2), Color3);
                spriteBatch.DrawString(spriteFont, bgtext, new Vector2(Position.X + 2, Position.Y + 2), Color3);
                spriteBatch.DrawString(spriteFont, bgtext, new Vector2(Position.X, Position.Y + 4), Color3);
                spriteBatch.DrawString(spriteFont, bgtext, new Vector2(Position.X+2, Position.Y + 4), Color3);
                spriteBatch.DrawString(spriteFont, text, Position2, Color2);
                spriteBatch.DrawString(spriteFont, text, Position, Color);
                spriteBatch.End();
            }
        }
    }
