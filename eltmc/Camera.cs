using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



using Microsoft.Xna.Framework.Graphics;

using Microsoft.Xna.Framework;

namespace CustomModelEffect
{
    class Camera
    {

        Matrix view;
        Matrix projection;

        Vector3 lookAt;

        Vector3 pos;
        Matrix rot;

        GraphicsDevice graphicsDevice;

        public Camera(GraphicsDevice graphicsDevice, Vector3 position, Vector3 lookAtVec, ScreenSplit screenSplit)
        {
            this.graphicsDevice = graphicsDevice;

            this.pos = position;
            this.rot = Matrix.CreateTranslation(Vector3.Zero);

            this.lookAt = lookAtVec;

            view = Matrix.CreateLookAt(pos, this.lookAt, Vector3.Up);
            

            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, graphicsDevice.Viewport.AspectRatio * (screenSplit == ScreenSplit.two ? 2 : 1), 0.01f, 1600); //do a blog entry on the near plane being 0.01 and 1;


        }

        public Vector3 Position
        {
            get{return pos;}

            set
            {
                this.pos = value;
                this.view = Matrix.CreateLookAt(pos, lookAt, Vector3.Up);
            }
        }

        public Matrix Rotation
        {
            get { return rot; }
            set 
            { 
                this.rot = value;
                this.lookAt = Vector3.Transform(Vector3.One, rot);
                this.view = Matrix.CreateLookAt(pos, lookAt, Vector3.Up);
            }
        }


        public Vector3 LookAt
        {
            get { return lookAt; }
            set
            {
                this.lookAt = value;
                this.view = Matrix.CreateLookAt(pos, lookAt, Vector3.Up);
            }
        }


        //hack TODO remove
        public Matrix Projection
        {
            get { return this.projection; }
            set { this.projection = value; }
        }



        public Matrix getView()
        {
            return this.view;
        }

        public Matrix getProjection()
        {
            return this.projection;
        }
    }
}
