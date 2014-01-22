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
    class CollisionEvent
    {
        public Entity entity;
        public Vector3 resolveVector;

        public CollisionEvent(Entity e, Vector3 r)
        {
            this.entity = e;
            this.resolveVector = r;
        }


    }
}
