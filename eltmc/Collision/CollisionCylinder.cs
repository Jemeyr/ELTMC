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

namespace CustomModelEffect.Collision
{
    class CollisionCylinder : CollisionVolume
    {
        public CollisionCylinder(CompositeCollision parent, float height, float radius, Vector3 offsetPosition)
            : base (parent, offsetPosition)
        {
            
            this.Height = height /2f;
            this.Radius = radius;
        }

        
        public float Radius
        {
            get {
                if (this.parent == null || this.parent.Entity == null)
                {
                    return this.dimensions.X;
                }
                else
                {
                    return this.dimensions.X *  this.parent.Entity.Model.Scale;
                }  
            
            }
            set{this.dimensions.Z = value; this.dimensions.X = value;}
        }

        public override Vector3 intersects(CollisionVolume other)
        {
            if (other.GetType() == typeof(CollisionCylinder))
            {
                return intersects((CollisionCylinder)other);
            }
            else
            {
                return intersects((CollisionBox)other);
            }
        }

        public Vector3 intersects(CollisionCylinder other)
        {
            float yDiff = this.position.Y - other.position.Y;
            float combinedHeight = this.Height + other.Height;
            float combinedRadius = this.Radius + other.Radius;

            //first check if height intersects
            if (Math.Abs(yDiff) < combinedHeight )
            {
                Vector2 xzDiff = new Vector2(this.position.X - other.position.X,this.position.Z - other.position.Z);
                if (xzDiff.LengthSquared() < combinedRadius * combinedRadius)
                {
                    float intersectY = combinedHeight - yDiff;
                    Vector2 intersectXZ = Vector2.Normalize(xzDiff) * (combinedRadius - xzDiff.Length());


                    if (intersectY * intersectY < intersectXZ.LengthSquared())
                    {
                        return  Vector3.Up * (combinedHeight - yDiff);
                    }
                    else
                    {
                        return new Vector3(intersectXZ.X,0f, intersectXZ.Y);
                    }
                }

            }
            return Vector3.Zero;
        }

        public Vector3 intersects(CollisionBox other)
        {
            

            float yDiff = this.position.Y - other.position.Y;


            float combinedHeight = this.Height + other.Height;
            Vector2 combinedDimensions = new Vector2(this.Radius + other.Dimensions.X, this.Radius + other.Dimensions.Z);

            //first check if height intersects
            if (Math.Abs(yDiff) < combinedHeight)
            {
                Vector2 xzDiff = new Vector2(this.position.X - other.position.X, this.position.Z - other.position.Z);

                float rot = MathHelper.Clamp(other.rotation, (float)-Math.PI, (float)Math.PI);//was pi/2 to -pi/2;

                //rotate by other
                if (other.rotation != 0)
                {
                    xzDiff = Vector2.Transform(xzDiff, Matrix.CreateRotationZ(rot));
                }
                    
                

                if (Math.Abs(xzDiff.X) < combinedDimensions.X && Math.Abs(xzDiff.Y) < combinedDimensions.Y)
                {
                    float intersectY = combinedHeight - Math.Abs(yDiff);
                    
                    Vector2 intersectXZ = new Vector2(combinedDimensions.X - Math.Abs(xzDiff.X), combinedDimensions.Y - Math.Abs(xzDiff.Y)); 
                    
                    
                    float intersectXZMin = intersectXZ.X < intersectXZ.Y ? intersectXZ.X : intersectXZ.Y;

                    if (intersectY < intersectXZMin)
                    {
                        return new Vector3(0f, intersectY * Math.Sign(yDiff), 0f);
                    }
                    else
                    {

                        if (intersectXZ.X > intersectXZ.Y)
                        {
                            intersectXZ.X = 0f;
                        }
                        else
                        {
                            intersectXZ.Y = 0f;
                        }

                        intersectXZ = new Vector2(intersectXZ.X * Math.Sign(xzDiff.X), intersectXZ.Y * Math.Sign(xzDiff.Y));

                        
                        intersectXZ = Vector2.Transform(intersectXZ, Matrix.CreateRotationZ(-rot));
                        xzDiff = Vector2.Transform(xzDiff, Matrix.CreateRotationZ(-rot));


                        return new Vector3(intersectXZ.X, 0f, intersectXZ.Y);
                    }
                }

            }
            return Vector3.Zero;

            
        }
    }
}
