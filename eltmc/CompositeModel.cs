using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using CustomModelEffect.Collision;

namespace CustomModelEffect
{
    class CompositeModel
    {
        Model model;
        Entity parent;

        public ModelMeshCollection Meshes;

        Color customColor;

        static Random r;

        float scale;


        private CompositeCollision collisionCollection;

        Vector3 position;
        Matrix rotation;


        public CompositeModel(Model model, Vector3 position, Matrix rotation)
        {
            this.model = model;
            this.Meshes = model.Meshes;
            this.position = position;
            this.rotation = rotation;

            //findme
            if (r == null)
            {
                r = new Random();
            }




            this.customColor = new Color((float)r.NextDouble(), (float)r.NextDouble(), (float)r.NextDouble());

            this.scale = 1.0f;


            this.collisionCollection = new CompositeCollision(this.parent,this.position, this.rotation);
        }

        public CompositeModel(Model model, Vector3 position, Matrix rotation, Color color)
            : this(model, position, rotation)
        {
            this.customColor = color;
        }

        public void setEntity(Entity e)
        {
            this.parent = e;
            this.collisionCollection.Entity = e;
        }

        public void newColor()
        {
            if (r == null)
            {
                r = new Random();
            }
            this.customColor = new Color((float)r.NextDouble(), (float)r.NextDouble(), (float)r.NextDouble());
            
        }

        public Matrix getWorld()
        {
            return Matrix.CreateScale(scale)* rotation * Matrix.CreateTranslation(position);
        }

        public Vector3 Position
        {
            get { return this.position; }
            set { this.position = value; this.collisionCollection.Position = value; }
        }

        public Matrix Rotation
        {
            get { return this.rotation; }
            set { 
                this.rotation = value; 
                Vector3 rot = Vector3.Transform(Vector3.Backward, value);
                this.collisionCollection.Rotation  = (float)Math.Atan2(this.rotation.M13 , this.rotation.M11); ;
            }
        }

        public float Scale
        {
            get { return this.scale; }
            set { this.scale = value;  }
        }

        public Color CustomColor
        {
            get { return this.customColor; }
            set { this.customColor = value; }
        }

        

        public void addCollisionBox(Vector3 dimensions, Vector3 offsetPosition, float yaw)
        {
            this.collisionCollection.addBox( dimensions, offsetPosition, yaw);
        }

        public void addCollisionCylinder(float height, float radius, Vector3 offsetPosition)
        {
            this.collisionCollection.addCylinder(height, radius, offsetPosition);
        }

        public void addBoundingCylinder(float height, float radius, Vector3 offsetPosition)
        {
            this.collisionCollection.addBoundingVolume(height, radius, offsetPosition);
        }

        public CollisionEvent intersects(CompositeModel other)
        {
            return this.collisionCollection.intersects(other.collisionCollection);
        }

        public string debug()
        {
            return this.position + " : " + this.collisionCollection.Position;
        }

    }
}
