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
    class CompositeCollision
    {
        private Entity parent;
        private CollisionVolume BoundingVolume;
        private List<CollisionVolume> volumes;
        private Vector3 position;
        private float yaw;

        public CompositeCollision(Entity entity, Vector3 position, Matrix rotation)
        {

            this.parent = entity;
            this.position = position;

            
            this.yaw = (float)Math.Atan2(rotation.M13, rotation.M11);//33,31

            volumes = new List<CollisionVolume>();
        }

        public void addBoundingVolume(float height, float radius, Vector3 offsetPosition)
        {
            this.BoundingVolume = new CollisionCylinder(this, height, radius, offsetPosition);
        }


        public void addBox(Vector3 dimensions, Vector3 offsetPosition, float yaw)
        {
            this.volumes.Add(new CollisionBox(this, dimensions, offsetPosition,yaw));
        }

        public void addCylinder(float height, float radius, Vector3 offsetPosition)
        {
            this.volumes.Add(new CollisionCylinder(this,height,radius,offsetPosition));
        }


        public CollisionEvent intersects(CompositeCollision other)
        {
            if (this.BoundingVolume.intersects(other.BoundingVolume) == Vector3.Zero)
            {
                return null;
            }
            

            Vector3 resolve = Vector3.Zero;
            foreach (CollisionVolume selfVolume in volumes)
            {
                
                foreach (CollisionVolume otherVolume in other.volumes)
                {
                    resolve += selfVolume.intersects(otherVolume);
                }
            }


            if (resolve != Vector3.Zero)
            {
                return new CollisionEvent(this.parent, resolve);
            }
            
            
            return null;

        }


        public Vector3 Position
        {
            get { return this.position; }
            set { this.position = value;}
        }
        
        public float Rotation
        {
            get { return this.yaw; }
            set
            {
                this.yaw = value;
            }
        }

        public Entity Entity
        {
            set { this.parent = value; }
            get { return this.parent; }
        }

        

    }
}
