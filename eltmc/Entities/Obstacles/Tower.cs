using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using CustomModelEffect.Collision;

namespace CustomModelEffect.Obstacles
{
    class Tower : Obstacle
    {
        const float TOWERHEIGHT = 485.75f;//166.5f;

        public Tower(CompositeModel cm)
            : base(cm)
        {
            this.model.addBoundingCylinder(TOWERHEIGHT + 50, 175, Vector3.Up *(25 + TOWERHEIGHT/2) );

            this.model.addCollisionCylinder(TOWERHEIGHT - 20, 110f, Vector3.Up * (TOWERHEIGHT - 20)/2f);
            this.model.addCollisionCylinder(20, 140, Vector3.Up * (TOWERHEIGHT - 10f));
         
            float thetaOffset = -0.0125f;
            float dist = 131f;

            //this.model.addCollisionBox(new Vector3(17, 28, 52.5f), new Vector3(dist * (float)Math.Cos(thetaOffset), TOWERHEIGHT, dist* (float)Math.Sin(thetaOffset)), thetaOffset);

            for (int i = 0; i <8; i++)
            {
                //higher sections
                this.model.addCollisionBox(new Vector3(17, 28, 52.5f), new Vector3(dist * (float)Math.Cos(thetaOffset + (float)Math.PI / 4f * i), TOWERHEIGHT, dist * (float)Math.Sin(thetaOffset + (float)Math.PI / 4f * i)), thetaOffset - (float)Math.PI / 4 * i + (float)Math.PI);

                //lower sections
                this.model.addCollisionBox(new Vector3(17, 14, 52.5f), new Vector3(dist * (float)Math.Cos(thetaOffset + (float)Math.PI / 4f * i + (float)Math.PI / 8), TOWERHEIGHT, dist * (float)Math.Sin(thetaOffset + (float)Math.PI / 4f * i + (float)Math.PI / 8)), thetaOffset - (float)Math.PI / 4 * i + (float)Math.PI - (float)Math.PI / 8);
            }

        }
    }
}
