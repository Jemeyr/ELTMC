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
    class KeyboardController : Controller
    {

        private KeyboardState ks;

        public override void update()
        {
            ks = Keyboard.GetState();
        }

        public override bool jump()
        {
            return ks.IsKeyDown(Keys.Space);
        }

        public override bool shoot()
        {
            return ks.IsKeyDown(Keys.LeftShift);
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
                    return ks.IsKeyDown(Keys.Up);
                case Controller.Direction.Down:
                    return ks.IsKeyDown(Keys.Down);
                case Controller.Direction.Right:
                    return ks.IsKeyDown(Keys.Right);
                case Controller.Direction.Left:
                    return ks.IsKeyDown(Keys.Left);
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
