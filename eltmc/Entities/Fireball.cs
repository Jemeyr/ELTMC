using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using CustomModelEffect.Entities;
using CustomModelEffect.Obstacles;
using CustomModelEffect.Collision;

namespace CustomModelEffect
{
    class Fireball : Entity, Updatable
    {
        private Vector3 velocity;
        private Player owner;

        public Fireball(Vector3 vel, Player owner, CompositeModel model) : base(model)
        {
            this.velocity = vel;
            this.owner = owner;
            this.model = model;

            this.model.addCollisionCylinder(2f, 1f, Vector3.Up * 0.5f);
            this.model.addBoundingCylinder(2f, 1f, Vector3.Up * 0.5f);
        }

        public void update()
        {
            Vector3 newPos = this.model.Position + velocity;

            
            if (newPos.X * newPos.X + newPos.Z * newPos.Z < 800 * 800 && newPos.Y > -200 && newPos.Y < 1500)
            {
                this.model.Position = newPos;

                foreach (Collidable cm in this.world.getCollidable())
                {
                    if (this.model.intersects(cm.Model) != null)
                    {
                        Player player = cm as Player;

                        if (player != null)
                        {

                            if (player != this.owner)
                            {
                                player.damage++;
                                Vector2 vec = new Vector2(player.damage  * this.velocity.X, player.damage *this.velocity.Z);

                                

                                player.wizVel =  (0.25f * player.damage);
                                player.wizXZvel += vec * 0.15f *this.model.Scale;//,maybe make this smaller later TODO
                               
                            }
                            else
                            {
                                continue;
                            }

                        }

                        TestBox t = cm as TestBox;

                        if (t != null)
                        {
                            t.targetHeight += this.model.Scale * 20f;
                        }
                        

                        

                        world.remove(this);
                        break;
                    }
                }

                
            }
            else
            {
                world.remove(this);
            }
        }


        public World WorldAccessor
        {
            get { return this.world; }
            set { this.world = value; }
        }


    }
}
