using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using CustomModelEffect.Collision;
using CustomModelEffect.Entities;

namespace CustomModelEffect.Obstacles
{
    class TestBox : Obstacle, Updatable
    {
        public float targetHeight;

        public TestBox(CompositeModel cm)
            : base(cm)
        {
            this.model.addCollisionBox(new Vector3(10f, 5f, 10f), Vector3.Up * 2.5f, 0);
            this.model.addBoundingCylinder(5f, 8f, Vector3.Up * 2.5f);
            this.targetHeight = this.model.Position.Y;
        }

        public void update()
        {
            if (this.targetHeight != this.model.Position.Y)
            {
                if (this.targetHeight - this.model.Position.Y > 1.5f)
                {
                    this.model.Position = this.model.Position + Vector3.Up * 1.5f;
                }
                else
                {
                    this.model.Position = this.model.Position + Vector3.Up * (this.targetHeight - this.model.Position.Y) * .015f;
                }
            }
        }

    }
}
