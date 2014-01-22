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


namespace CustomModelEffect.Control
{
    class KeyboardMouseController : Controller
    {

        private KeyboardState ks;
        private MouseState ms;
        private Vector2 pos,oldPos;
        

        public KeyboardMouseController()
        : base()
        {
            Mouse.SetPosition(200, 200);
        }

        public override void update()
        {
            
            ks = Keyboard.GetState();
            ms = Mouse.GetState();

            oldPos = new Vector2(200);// pos;
            
            pos = new Vector2(ms.X, ms.Y);
            
            Mouse.SetPosition(200, 200);
            
        }


        public override bool jump()
        {
            return ks.IsKeyDown(Keys.Space);
        }

        public override bool shoot()
        {
            return ms.LeftButton == ButtonState.Pressed;
        }

        public override bool move(Controller.Direction d)
        {
            switch (d)
            {
                case Controller.Direction.Up:
                    return ks.IsKeyDown(Keys.W);
                case Controller.Direction.Down:
                    return ks.IsKeyDown(Keys.S);
                case Controller.Direction.Right:
                    return ks.IsKeyDown(Keys.D);
                case Controller.Direction.Left:
                    return ks.IsKeyDown(Keys.A);
                default:
                    return false;
            }

        }

        public override bool look(Controller.Direction d)
        {
            switch (d)
            {
                case Controller.Direction.Up:
                    return pos.Y < oldPos.Y;
                case Controller.Direction.Down:
                    return pos.Y > oldPos.Y;
                case Controller.Direction.Right:
                    return pos.X > oldPos.X + 1;
                case Controller.Direction.Left:
                    return pos.X < oldPos.X - 1;
                default:
                    return false;
            }

            

        }

        public override bool debug()
        {
            return (ks.IsKeyDown(Keys.G));
        }


    }
}
