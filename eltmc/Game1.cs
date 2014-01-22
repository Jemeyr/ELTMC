using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using CustomModelEffect.Control;

using CustomModelEffect.Obstacles;

namespace CustomModelEffect
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;


        Vector3 bounds;
        
        Effect sobelEffect;



        const float TOWERHEIGHT = 900;///187;
        const float TOWERRAD = 123f;
        const float TOWERRADSQUARED = TOWERRAD * TOWERRAD;

        const float ZOFFSET = 7.8f;
        const float XOFFSET = 0.35f;
        const float WIZARDSPEED = 1f;

        int FLOATINGBOXCOUNT;

        World world;
        Player playerOne,playerTwo,playerThree,playerFour;
        Obstacle tower;

        RenderMaster renderMaster;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);

            Components.Add(new FrameRateCounter(this,new Vector2(0,0), Color.Red, Color.Purple));

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            
            InitGraphicsMode(1366, 768, true);
            //InitGraphicsMode(1024, 768, false);
            bounds = new Vector3(.95f, .5f, .05f);



            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            ScreenSplit scMode = ScreenSplit.two;
            FLOATINGBOXCOUNT = 150;
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            sobelEffect = Content.Load<Effect>("Sobel2");

            renderMaster = new RenderMaster(scMode, GraphicsDevice, spriteBatch, sobelEffect,  bounds);

            world = new World(renderMaster);


            world.addModel("terrain", Content.Load<Model>("terrain"));
            world.addModel("tower", Content.Load<Model>("tower"));
            world.addModel("wiz", Content.Load<Model>("wiz"));
            world.addModel("fireball", Content.Load<Model>("fireball"));
            world.addModel("testbox", Content.Load<Model>("testbox"));

            tower = new Tower( new CompositeModel(world.getModel("tower"), new Vector3(0f, 0f, 0f), Matrix.CreateFromYawPitchRoll(0f, 0f, 0f)));


            float dist = 300f;
            for (int i = 0; i < 8; i++)
            {
                world.add(new Tower(new CompositeModel(world.getModel("tower"),new Vector3(
                            (dist  + i % 2 * (dist / 2)) * (float)Math.Sin(i * (Math.PI / 4) ),
                            -20 * (1 + i), 
                            (dist  + i % 2 * (dist / 2)) * (float)Math.Cos(i * (Math.PI / 4) )), 
                            Matrix.CreateRotationZ(0f))));
            }

        

            Random r = new Random();
            Color c;
            TestBox t;
            for (int i = 0; i < FLOATINGBOXCOUNT; i++)
            {

                c = new Color((int)(r.NextDouble() * 255), (int)(r.NextDouble() * 255), (int)(r.NextDouble() * 255));
                t = new TestBox(new CompositeModel(world.getModel("testbox"), new Vector3(-500 + r.Next(1000),  r.Next(800), -500f + r.Next(1000)), Matrix.CreateRotationY((float)(r.NextDouble() * Math.PI)),c));

                world.add(t);
            }


            Controller p1 = new XboxController(PlayerIndex.One);
            Controller p2 = new KeyboardController();       //XboxController(PlayerIndex.Two);
            Controller p3 = new XboxController(PlayerIndex.Three);////XboxController(PlayerIndex.Two);
            Controller p4 = new XboxController(PlayerIndex.Four);// new KeyboardController();//XboxController(PlayerIndex.Two);


            world.add(p1);
            world.add(p2);
            world.add(p3);
            world.add(p4);
            
            Color c1 = new Color((int)(r.NextDouble() * 255), (int)(r.NextDouble() * 255), (int)(r.NextDouble() * 255));
            Color c2 = new Color((int)(r.NextDouble() * 255), (int)(r.NextDouble() * 255), (int)(r.NextDouble() * 255));
            Color c3 = new Color((int)(r.NextDouble() * 255), (int)(r.NextDouble() * 255), (int)(r.NextDouble() * 255));
            Color c4 = new Color((int)(r.NextDouble() * 255), (int)(r.NextDouble() * 255), (int)(r.NextDouble() * 255));


            playerOne = new Player(new CompositeModel(world.getModel("wiz"), new Vector3(0f, (float)TOWERHEIGHT - 350, 30f), Matrix.CreateFromYawPitchRoll(45f, 0f, 0f), c1),
                new Camera(graphics.GraphicsDevice, new Vector3(50, 120, 50), new Vector3(0f, 63f, 0f), scMode),p1);// 

            playerTwo = new Player(new CompositeModel(world.getModel("wiz"), new Vector3(0f, (float)TOWERHEIGHT - 350, -30f), Matrix.CreateFromYawPitchRoll(0f, 0f, 0f), c2),
                new Camera(graphics.GraphicsDevice, new Vector3(50, 120, 50), new Vector3(0f, 63f, 0f), scMode), p2);

            playerThree = new Player(new CompositeModel(world.getModel("wiz"), new Vector3(30f, (float)TOWERHEIGHT - 350, 0f), Matrix.CreateFromYawPitchRoll(0f, 0f, 0f), c3),
                new Camera(graphics.GraphicsDevice, new Vector3(50, 120, 50), new Vector3(0f, 63f, 0f), scMode), p1);
            
            playerFour = new Player(new CompositeModel(world.getModel("wiz"), new Vector3(-30f, (float)TOWERHEIGHT - 350, 0f), Matrix.CreateFromYawPitchRoll(0f, 0f, 0f), c4),
                new Camera(graphics.GraphicsDevice, new Vector3(50, 120, 50), new Vector3(0f, 63f, 0f), scMode), p1);
            

            
            

            world.add(playerOne);
            world.add(playerTwo);
            //world.add(playerThree);//
            //world.add(playerFour);


            world.add(tower);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {

            KeyboardState keyState = Keyboard.GetState();
            // Allows the game to exit
            if (keyState.IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }

            world.update();

            base.Update(gameTime);
        }




        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            
            //*/
            renderMaster.changeRenderList("_master");



            Camera[] cams = new Camera[4];

            cams[0] = playerOne.Cam;
            cams[1] = playerTwo.Cam;
            cams[2] = playerThree.Cam;
            cams[3] = playerFour.Cam;

            //some hacky code to change render during runtime, ignores the nice classes for input I made =(


            KeyboardState hackKeyboard = Keyboard.GetState();

            bool hackPP = true;
            int hackTar = 0;

            if (hackKeyboard.IsKeyDown(Keys.NumPad0))
            {
                hackPP = false;
            }
            if (hackKeyboard.IsKeyDown(Keys.NumPad1))
            {
                hackTar = -1; hackPP = false;
            }
            if (hackKeyboard.IsKeyDown(Keys.NumPad2))
            {
                hackTar = 1; hackPP = false;
            }

            renderMaster.render(cams, hackPP, hackTar);

            base.Draw(gameTime);
        }




        /// <summary>
        /// Attempt to set the display mode to the desired resolution.  Itterates through the display
        /// capabilities of the default graphics adapter to determine if the graphics adapter supports the
        /// requested resolution.  If so, the resolution is set and the function returns true.  If not,
        /// no change is made and the function returns false.
        /// </summary>
        /// <param name="iWidth">Desired screen width.</param>
        /// <param name="iHeight">Desired screen height.</param>
        /// <param name="bFullScreen">True if you wish to go to Full Screen, false for Windowed Mode.</param>
        private bool InitGraphicsMode(int iWidth, int iHeight, bool bFullScreen)
        {
            // If we aren't using a full screen mode, the height and width of the window can
            // be set to anything equal to or smaller than the actual screen size.
            if (bFullScreen == false)
            {
                if ((iWidth <= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width)
                    && (iHeight <= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height))
                {
                    graphics.PreferredBackBufferWidth = iWidth;
                    graphics.PreferredBackBufferHeight = iHeight;
                    graphics.IsFullScreen = bFullScreen;
                    graphics.ApplyChanges();
                    return true;
                }
            }
            else
            {
                // If we are using full screen mode, we should check to make sure that the display
                // adapter can handle the video mode we are trying to set.  To do this, we will
                // iterate thorugh the display modes supported by the adapter and check them against
                // the mode we want to set.
                foreach (DisplayMode dm in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
                {
                    // Check the width and height of each mode against the passed values
                    if ((dm.Width == iWidth) && (dm.Height == iHeight))
                    {
                        // The mode is supported, so set the buffer formats, apply changes and return
                        graphics.PreferredBackBufferWidth = iWidth;
                        graphics.PreferredBackBufferHeight = iHeight;
                        graphics.IsFullScreen = bFullScreen;
                        graphics.ApplyChanges();
                        return true;
                    }
                }
            }
            return false;
        }


    }
}
