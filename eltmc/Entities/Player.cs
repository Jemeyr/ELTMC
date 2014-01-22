using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using CustomModelEffect.Collision;
using CustomModelEffect.Control;
using CustomModelEffect.Entities;


namespace CustomModelEffect
{
    class Player : Entity, Collidable, Updatable
    {
        Color color;
        Camera cam;
        Controller controller;

        bool charging;
        float power;

        //this represents a pitch which isn't a rotation of the model, but the direction of the camera/shots
        float bearing;


        //movement/bool for jump
        public float wizVel;
        public Vector2 wizXZ;
        public Vector2 wizXZvel;
        bool wizFloat;


        //speedova wizard
        const float WIZARDJUMP = 1.6f;//1.1f
        const float WIZARDSPEED = 0.9f;//0.5
        const float TERMINALVELOCITY = -1.8f;
        const float GRAVITYPOWERFUL = -0.05f;
        const float GRAVITYWEAK = -0.0375f;
        const float WIZTURN = 0.07f;
        const float WIZPITCH = 0.025f;

        float AAbearing = 0.4f;
        float AArot = 0.966f;

        public int damage;

        public Player(CompositeModel model, Camera camera, Controller controller) : base(model)
        {
            this.damage = 0;

            this.cam = camera;
            this.controller = controller;

            this.wizXZ = Vector2.Zero;
            this.wizXZvel = Vector2.Zero;

            this.wizVel = 0f;
            this.wizFloat = true;

            this.power = 1f;
            this.charging = false;

            this.color = this.model.CustomColor;

            bearing = 0;


            this.model.addCollisionCylinder(6.1f, 1f, Vector3.Up * 3.05f);
            this.model.addBoundingCylinder(6.1f, 1f, Vector3.Up * 3.05f);

        }

        public void update()
        {
            //shortcut to allow null controllers to be static models
            if (controller == null )
            {
                return;
            }


            List<Collidable> collidables = this.world.getCollidable();

            this.wizXZvel *= 0.99f;
            

            Vector3 newPos = model.Position;
            Vector3 oldPos = model.Position;

            if (controller.move(Controller.Direction.Up))
            {
                newPos += Vector3.Transform(Vector3.Backward * WIZARDSPEED, model.Rotation);
            }

            if (controller.move(Controller.Direction.Down))
            {
                newPos += Vector3.Transform(Vector3.Forward * WIZARDSPEED, model.Rotation);
            }

            if (controller.move(Controller.Direction.Left))
            {
                newPos += Vector3.Transform(Vector3.Right * WIZARDSPEED, model.Rotation);
            }

            if (controller.move(Controller.Direction.Right))
            {
                newPos += Vector3.Transform(Vector3.Left * WIZARDSPEED, model.Rotation);
            }


           // wizXZ = new Vector2(newPos.X - this.model.Position.X, newPos.Z - this.model.Position.Z);

           wizVel =  wizVel <= TERMINALVELOCITY ? TERMINALVELOCITY : wizVel > 0 ? wizVel + GRAVITYPOWERFUL : wizVel +GRAVITYWEAK;
            

            if (newPos.Y < -200f)
            {
                //you die!

               this.model.Position = new Vector3(0f, 550, 30f);
               this.model.newColor();
               this.color = this.model.CustomColor;
               this.damage = 0;
               this.bearing = 0;
                //world.remove(this);
                return;
            }


            if (controller.jump() && !wizFloat)
            {
                wizVel = WIZARDJUMP;//1.1f;
                wizFloat = true;
            }
            

            newPos += Vector3.Up * wizVel;
            
            //update pos if allowed
            //model.Position = newPos;
            model.Position = newPos;
            model.Position += new Vector3(wizXZ.X + this.wizXZvel.X, 0f, wizXZ.Y + this.wizXZvel.Y);

            Vector3 resolve = Vector3.Zero;

            this.wizFloat = true;
            
            foreach (Collidable cm in collidables)
            {
                Entity e = cm as Entity;

                if (this.Equals(e))
                {
                    continue;
                }

                
                CollisionEvent ce = this.model.intersects(e.Model);
                
                if (ce != null)
                {
                    


                    if (ce.resolveVector.Y != 0)
                    {
                        
                        this.wizVel = 0f;
                        if (ce.resolveVector.Y > 0)
                        {
                            this.wizXZvel = Vector2.Zero;
                            this.wizFloat = false;
                        }
                    }
                    
                    //add to resolution vector
                    resolve += ce.resolveVector;
                }
                
            }
            model.Position += resolve;

            if (controller.look(Controller.Direction.Left))
            {
                model.Rotation *= Matrix.CreateFromYawPitchRoll(WIZTURN, 0f, 0f);
            }
            if (controller.look(Controller.Direction.Right))
            {
                model.Rotation *= Matrix.CreateFromYawPitchRoll(-WIZTURN, 0f, 0f);
            }

            
            if (controller.look(Controller.Direction.Up))
            {
               bearing -= WIZPITCH;
            }
            if (controller.look(Controller.Direction.Down))
            {
                bearing += WIZPITCH;
            }
            bearing = MathHelper.Clamp(bearing, -(float)Math.PI/2.01f, (float)Math.PI/2.01f);

            foreach (Entity e in this.world.getCollidable())
            {
                Player p = e as Player;
                if (p != null && p != this)
                {
                    Vector3 diff = (p.model.Position - this.model.Position);
                    
                    diff.Y = 0;

                    float diffLength = diff.Length();
                    diff = Vector3.Normalize(diff);

                    Vector3 aim = Vector3.Normalize(Vector3.Transform(Vector3.Backward, this.model.Rotation));

                    float incidence = Vector3.Dot(diff, aim);
                    if (incidence > AArot)
                    {
                        float amount = 0.95f;

                        this.model.Rotation = Matrix.CreateFromYawPitchRoll((float)Math.Atan2(MathHelper.Lerp(diff.X, aim.X, amount), MathHelper.Lerp(diff.Z, aim.Z, amount)), 0f, 0f);

                        float diffBearing = (float)Math.Atan2((float)p.model.Position.Y - (this.model.Position.Y), diffLength);
                        if (Math.Abs(diffBearing - bearing) < AAbearing)
                        {
                            bearing = MathHelper.Lerp(-diffBearing, bearing, amount);
                        }
                        break;
                    }
                }
            }
            //autoaim?

            if (controller.debug())
            {
                AArot = 0f;
                AAbearing = 99f;
            }
            else
            {
                AArot = 0.996f;
                AAbearing = 0.4f;
            }

            //update camera position
            Vector3 camTargetPosition = this.model.Position + Vector3.Up * 10 + Vector3.Transform(Vector3.Forward * 12, Matrix.Multiply(Matrix.CreateRotationX(bearing / 1.5f), this.model.Rotation));
            cam.Position = cam.Position + (camTargetPosition - cam.Position) * 0.28f;
            
            cam.LookAt = this.model.Position + Vector3.Up * 8  + (Vector3.Transform(Vector3.Backward * 6, Matrix.Multiply(Matrix.CreateRotationX(bearing), this.model.Rotation))) / 2;

            if (controller.shoot())
            {

                if (controller.debug())
                {
                    this.power = 20f;
                }

                this.power *= this.power > 20f? 1.0001f : 1.03f;


                if (this.power > 20f)
                {

                    this.model.CustomColor = new Color(this.model.CustomColor.R + 1, this.model.CustomColor.G + 1, this.model.CustomColor.B + 1);
                }

                if (!this.charging)
                    this.charging = true;

                


            }
            else
            {
                if (this.charging)
                {

                    this.model.CustomColor = this.color;

                    Fireball f = new Fireball(Vector3.Transform(Vector3.Backward * 3f, Matrix.Multiply(Matrix.CreateRotationX(bearing), this.model.Rotation)),
                        this, new CompositeModel(this.world.getModel("fireball"), Vector3.Transform(Vector3.Up * ( 3.5f + power ) + Vector3.Backward * 1.5f, this.model.getWorld()), Matrix.CreateRotationY(0), this.model.CustomColor));
                    f.Model.Scale = power;
                    this.world.add(f);
                    
                    this.charging = false;
                    this.power = 1;
                }
            }
            

            
        }

        
        public Camera Cam
        {
            get{return this.cam;}
            set{this.cam = value;}
        }

        public World WorldAccessor
        {
            get { return this.world; }
            set { this.world = value; }
        }


    }
}
