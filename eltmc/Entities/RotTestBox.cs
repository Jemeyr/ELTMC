using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using CustomModelEffect.Collision;
using CustomModelEffect.Entities;

namespace CustomModelEffect.Obstacles
{
    class RotTestBox : Entity, Updatable, Collidable 
    {

        public RotTestBox(CompositeModel cm)
            : base(cm)
        {
            this.model.addCollisionBox(new Vector3(10f,5f,10f), Vector3.Up * 2.5f, 0);
        }

        public void update()
        {
            this.model.Rotation *= Matrix.CreateRotationY((float)Math.PI / 1000);
        }

    }
}
