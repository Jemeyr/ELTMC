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
    class CollisionBox : CollisionVolume
    {
        public CollisionBox(CompositeCollision parent, Vector3 dimensions, Vector3 offsetPosition, float yaw)
            : base (parent, offsetPosition, yaw)
        {
            this.dimensions = new Vector3(dimensions.X /2f, dimensions.Y /2f, dimensions.Z /2f);
        }


        public Vector3 Dimensions
        {
            get {
                if (this.parent == null || this.parent.Entity == null)
                {
                    return this.dimensions;
                }
                else
                {
                    return this.dimensions *  this.parent.Entity.Model.Scale;
                }
            
            }
        }

        public override Vector3 intersects(CollisionVolume other)
        {
            if(other.GetType() == typeof(CollisionCylinder))
            {
                return intersects((CollisionCylinder)other);
            }
            else
            {
                return intersects((CollisionBox)other);
            }
        }


        public Vector3 intersects(CollisionBox other)
        {
            float Xdist = Math.Abs(this.position.X - other.position.X); 
            float Ydist = Math.Abs(this.position.Y - other.position.Y); 
            float Zdist = Math.Abs(this.position.Z - other.position.Z);

            float Xdiff = this.dimensions.X + other.dimensions.X;
            float Ydiff = this.dimensions.Y + other.dimensions.Y;
            float Zdiff = this.dimensions.Z + other.dimensions.Z;

            //TODO: Fix this to account for rotation too
            if (Ydist > Ydiff && Xdist > Xdiff && Zdist < Zdiff)
            {
                Vector3 thing = new Vector3(Xdist - Xdiff, Ydist - Ydiff, Zdist - Zdiff);
                return Vector3.Zero;//TODO FIXMEERROR
            }


            return Vector3.Zero;
        }

        public Vector3 intersects(CollisionCylinder other)
        {
            Console.Out.WriteLine("Don't use me!");
                        //TODO: Fix this to account for rotation too
            return Vector3.Zero;
        }

    }
}
