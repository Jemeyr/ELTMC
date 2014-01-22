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
    class XboxController : Controller
    {

        PlayerIndex playerIndex;
        GamePadState gamePadState;

        public XboxController(PlayerIndex playerIndex)
        {
            this.playerIndex = playerIndex;
        }

        public override void update()
        {
            this.gamePadState = GamePad.GetState(playerIndex);
        }
        
        public override bool jump()
        {
            return gamePadState.Buttons.A == ButtonState.Pressed;
        }

        public override bool shoot()
        {
            return gamePadState.Buttons.RightShoulder == ButtonState.Pressed || gamePadState.Triggers.Right >.5f;
        }

        public override bool move(Controller.Direction d)
        {
        switch (d)
            {
                case Controller.Direction.Up:
                    return gamePadState.ThumbSticks.Left.Y > 0.5f;
                case Controller.Direction.Down:
                    return gamePadState.ThumbSticks.Left.Y < -0.5f;
                case Controller.Direction.Right:
                    return gamePadState.ThumbSticks.Left.X > 0.5f;
                case Controller.Direction.Left:
                    return gamePadState.ThumbSticks.Left.X < -0.5f;
                default:
                    return false;
            }

        }

        public override bool look(Controller.Direction d)
        {
            switch (d)
            {
                case Controller.Direction.Up:
                    return gamePadState.ThumbSticks.Right.Y > 0.5f;
                case Controller.Direction.Down:
                    return gamePadState.ThumbSticks.Right.Y < -0.5f;
                case Controller.Direction.Right:
                    return gamePadState.ThumbSticks.Right.X > 0.5f;
                case Controller.Direction.Left:
                    return gamePadState.ThumbSticks.Right.X < -0.5f;
                default:
                    return false;
            }
        }

        public override bool debug()
        {
            return gamePadState.Buttons.Y == ButtonState.Pressed;
        }


    }
}
