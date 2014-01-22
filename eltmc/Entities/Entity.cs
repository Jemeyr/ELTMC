using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using CustomModelEffect.Collision;
using CustomModelEffect.Control;


namespace CustomModelEffect
{
    class Entity
    {
        protected World world;
        protected CompositeModel model;
        
        public Entity(CompositeModel model)
        {
            this.model = model;
            model.setEntity(this);
        }

        public CompositeModel Model
        {
            get { return this.model; }
            set { this.model = value; }
        }


        public World World
        {
            get { return this.world; }
            set { this.world = value; }
        }


    }
}
