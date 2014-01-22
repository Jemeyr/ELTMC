using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using CustomModelEffect.Entities;

namespace CustomModelEffect
{
    class Obstacle : Entity, Collidable
    {
        public Obstacle(CompositeModel cm)
            : base(cm) { }



    }
}
