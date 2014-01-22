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
    abstract class CollisionVolume
    {

        private Vector3 offsetPosition;
        private float offsetYaw;
        
        protected CompositeCollision parent;

        private Vector3 cachedRotatedOffsetPosition;
        private float cachedYaw;

        protected Vector3 dimensions;
        

        public CollisionVolume(CompositeCollision parent, Vector3 offsetPosition, float offsetYaw)
        {
            this.parent = parent;
            this.dimensions = new Vector3();

            this.offsetPosition = offsetPosition;
            this.offsetYaw = offsetYaw;

            this.cachedYaw = parent.Rotation;
            
            this.cachedRotatedOffsetPosition = new Vector3(
                offsetPosition.X * (float)Math.Cos((double)parent.Rotation) + offsetPosition.Z * (float)Math.Sin((double)parent.Rotation), 
                offsetPosition.Y,
                offsetPosition.Z * (float)Math.Cos((double)parent.Rotation) + offsetPosition.X * (float)Math.Sin((double)parent.Rotation));

        }

        public CollisionVolume(CompositeCollision parent, Vector3 offsetPosition)
            : this (parent, offsetPosition, 0f){ }


        public abstract Vector3 intersects(CollisionVolume other);

        public Vector3 position
        {
            get
            {

                if (parent.Rotation != this.cachedYaw)
                {
                    this.cachedYaw = parent.Rotation;
                    if (this.offsetPosition != Vector3.Zero)
                    {
                        this.cachedRotatedOffsetPosition = new Vector3(
                            offsetPosition.X * (float)Math.Cos((double)parent.Rotation) + offsetPosition.Z * (float)Math.Sin((double)parent.Rotation),
                            offsetPosition.Y,
                            offsetPosition.Z * (float)Math.Cos((double)parent.Rotation) + offsetPosition.X * (float)Math.Sin((double)parent.Rotation));
                    }
                }
                return this.parent.Position + this.cachedRotatedOffsetPosition;

            }
        }

        public float rotation
        {
            get {
                if (parent.Rotation != this.cachedYaw)
                {
                    this.cachedYaw = parent.Rotation;
                    if (this.offsetPosition != Vector3.Zero)
                    {
                        this.cachedRotatedOffsetPosition = new Vector3(
                            offsetPosition.X * (float)Math.Cos((double)parent.Rotation) + offsetPosition.Z * (float)Math.Sin((double)parent.Rotation),
                            offsetPosition.Y,
                            offsetPosition.Z * (float)Math.Cos((double)parent.Rotation) + offsetPosition.X * (float)Math.Sin((double)parent.Rotation));
                    }
                }
                return this.cachedYaw + this.offsetYaw; 
            }
        }
        
        public float Height
        {
            get {
                if (this.parent == null || this.parent.Entity == null)
                {
                    return this.dimensions.Y;
                }
                else
                {
                    return this.dimensions.Y * this.parent.Entity.Model.Scale;
                }
            }
            set 
            {
                this.dimensions.Y = value;
            }
        }

    }
}
