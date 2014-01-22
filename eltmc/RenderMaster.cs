using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace CustomModelEffect
{
    //An enum for how many viewports we're rendering
    public enum ScreenSplit { one = 1, two = 2, four = 4 };
    

    class RenderMaster
    {
        //Xna things for rendering
        GraphicsDevice graphicsDevice;
        SpriteBatch spriteBatch;
        
        //a bounds which is passed in to control how light intensity is bucketed
        Vector3 bounds;

        //a splitscreen enum
        ScreenSplit screenSplit;

        // This creates strings which map to lists of objects to render, so we can render different sets of 
        //objects (not used to its full potential here)
        Dictionary<string, List<CompositeModel>> renderObjectLists;

        // The current list to render
        List<CompositeModel> currentList;

        //Xna effect objects
        Effect sobelEffect;

        RenderTarget2D[] normalRenderTarget, depthRenderTarget, toonRenderTarget;
        private Rectangle[] screenRect;


        public RenderMaster(ScreenSplit option, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, Effect sobelEffect, Vector3 bounds)
        {        
            this.renderObjectLists = new Dictionary<string, List<CompositeModel>>();
            renderObjectLists.Add("_master", new List<CompositeModel>());
            currentList = renderObjectLists["_master"];

            this.screenSplit = option;
            
            this.bounds = bounds;

            this.graphicsDevice = graphicsDevice;
            this.spriteBatch = spriteBatch;

            //arrays of render targets, as to match number of screens to draw
            normalRenderTarget = new RenderTarget2D[(int)this.screenSplit];
            depthRenderTarget = new RenderTarget2D[(int)this.screenSplit];
            toonRenderTarget = new RenderTarget2D[(int)this.screenSplit];
            screenRect = new Rectangle[(int)this.screenSplit];


            //this creates a bunch of targets of the right size for rendering multiple viewports
            for (int i = 0; i < (int)this.screenSplit; i++)
            {
                screenRect[i] = new Rectangle(
                    (i / 2) * this.graphicsDevice.Viewport.Width / 2, 
                    (i % 2) * this.graphicsDevice.Viewport.Height / 2,
                    this.graphicsDevice.Viewport.Width / ((int)screenSplit == 4 ? 2 : 1), 
                    this.graphicsDevice.Viewport.Height / ((int)screenSplit == 1 ? 1 : 2));

                normalRenderTarget[i] = new RenderTarget2D(this.graphicsDevice, screenRect[i].Width, screenRect[i].Height, false, this.graphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);
                depthRenderTarget[i] = new RenderTarget2D(this.graphicsDevice, screenRect[i].Width, screenRect[i].Height, false, this.graphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);
                toonRenderTarget[i] = new RenderTarget2D(this.graphicsDevice, screenRect[i].Width, screenRect[i].Height, false, this.graphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);
                
            }


            this.sobelEffect = sobelEffect;
        }


        //poor name, this actually draws the list of models, from the perspective of a camera, to a certain render target, with a certain technique
        public void DrawModel(Camera camera, RenderTarget2D renderTarget, String techniqueName, Color bgColor)
        {

            graphicsDevice.SetRenderTarget(renderTarget);
            graphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };

            this.graphicsDevice.Clear(bgColor);

            //This big nasty iteration just goes over each of the models, their meshes and the effects to render them. It's the XNA way!
            foreach (CompositeModel model in currentList)
            {

                foreach (ModelMesh mesh in model.Meshes)
                {
                        foreach (Effect eff in mesh.Effects)
                        {
                            eff.CurrentTechnique = eff.Techniques[techniqueName];

                            Matrix World = model.getWorld();

                            eff.Parameters["World"].SetValue(World);
                            eff.Parameters["View"].SetValue(camera.getView());
                            eff.Parameters["Projection"].SetValue(camera.getProjection());

                            eff.Parameters["WorldInverseTranspose"].SetValueTranspose(Matrix.Invert(World));


                            eff.Parameters["Bounds"].SetValue(bounds);

                            //Oh yes, any transparent texture will take a custom color.
                            eff.Parameters["PlayerColor"].SetValue(model.CustomColor.ToVector4());

                            eff.CurrentTechnique.Passes[0].Apply();

                        }
                   //after all the effects are applied, we draw. I should batch up some geometry and do this less so I can render more.
                   mesh.Draw();
                }
            }
        }
        
        //kinda hacky int input for the target, we'll say negative is depth, positive is normal, and 0 is toon
        public void render(Camera[] cam, bool postProcess, int target)
        {
            
            //for each screen to render
            for (int i = 0; i < (int)screenSplit; i++)
            {
                //render three times per viewport, for normal, depth, and toon-shading
                DrawModel(cam[i], normalRenderTarget[i], "Normal", Color.Transparent);
                DrawModel(cam[i], depthRenderTarget[i], "Depth", Color.Transparent);
                DrawModel(cam[i], toonRenderTarget[i], "Toon", Color.CornflowerBlue);
            }

            //this draws the sky (null output will render to screen)
            this.graphicsDevice.SetRenderTarget(null);
            this.graphicsDevice.Clear(Color.Cornsilk);

            Vector2 resolution = new Vector2(screenRect[0].Width, screenRect[0].Height);

            sobelEffect.Parameters["ScreenResolution"].SetValue(resolution);
            sobelEffect.CurrentTechnique = sobelEffect.Techniques["EdgeDetect"];


            //this begins the spritebatch(xna thing to batch textured planes for easier 2d rendering)
            if (postProcess)
            {
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend,
                    SamplerState.LinearClamp, DepthStencilState.Default,
                    RasterizerState.CullNone, sobelEffect);//with the effect
            }
            else
            {
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend,
                    SamplerState.LinearClamp, DepthStencilState.Default,
                    RasterizerState.CullNone, null);//without
            }

            //draw the target
            for (int i = 0; i < (int)screenSplit; i++)
            {
                sobelEffect.Parameters["NormalTexture"].SetValue(normalRenderTarget[i]);
                sobelEffect.Parameters["DepthTexture"].SetValue(depthRenderTarget[i]);
                sobelEffect.Parameters["SceneTexture"].SetValue(toonRenderTarget[i]);

                // here we cheat to allow the user to control output, the user can specify which render to draw.
                // The post processing can occur over any render if necessary, but will always be the same as it always
                // has the normal and depth textures as its parameters for normal and depth texture
                // although changing this could allow me to pass in a blank texture as a parameter to see the contributions 
                // of the normal and depth map. (Every feature I add gives me 10 new ideas)
                if (target < 0)
                {
                    spriteBatch.Draw((Texture2D)depthRenderTarget[i], screenRect[i], Color.White);
                }
                else if (target > 0)
                {
                    spriteBatch.Draw((Texture2D)normalRenderTarget[i], screenRect[i], Color.White);
                }
                else
                {
                    spriteBatch.Draw((Texture2D)toonRenderTarget[i], screenRect[i], Color.White);
                }
                
            }
            
            spriteBatch.End();

        }

        // This is some unused infrastructure to allow me to render different sets of objects for different screens. Not used.
        public void addRenderList(string s)
        {
            this.renderObjectLists.Add(s, new List<CompositeModel>());
        }

        public void changeRenderList(string s)
        {
            this.currentList = renderObjectLists[s];
        }

        public void addModel(CompositeModel m)
        {
            renderObjectLists["_master"].Add(m);
        }

        public void addModel(string s, CompositeModel m)
        {
            renderObjectLists[s].Add(m);
        }


        public void removeModel(CompositeModel m)
        {
            foreach (KeyValuePair<string, List<CompositeModel>> entry in renderObjectLists)
            {
                entry.Value.Remove(m);
            }
        }

        public void removeModel(string s, CompositeModel m)
        {
            renderObjectLists[s].Remove(m);
        }

    }
}
