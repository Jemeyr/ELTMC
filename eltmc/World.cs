using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using CustomModelEffect.Entities;

using CustomModelEffect.Control;

namespace CustomModelEffect
{
    class World
    {
        private RenderMaster renderMaster;


        private List<Controller> controllers;
        
        private List<Entity> entities;

        private List<Collidable> collidables;
        private List<Updatable> updatables;

        

        private Dictionary<string, Model> modelPool;

        public World(RenderMaster renderMaster)
        {
            //this.fireballs = new List<Fireball>();
            //this.players = new List<Player>();
            //this.obstacles = new List<Obstacle>();


            this.collidables = new List<Collidable>();
            this.updatables = new List<Updatable>();

            this.entities = new List<Entity>();

            this.controllers = new List<Controller>();

            this.modelPool = new Dictionary<string, Model>();
            this.renderMaster = renderMaster;
           
        }


        public void addModel(string s, Model m)
        {
            this.modelPool.Add(s, m);
            CompositeModel waste = new CompositeModel(m, new Vector3(9999), Matrix.CreateRotationY(0f));
            this.renderMaster.addModel(waste);
            waste = null;
        }

        public Model getModel(string s)
        {
            return this.modelPool[s];
        }

        public void update()
        {
            
            for (int i = 0; i < updatables.Count; i++)
            {
                updatables[i].update();
            }

            foreach (Controller controller in controllers)
            {
                controller.update();
            }

        }


        public List<Collidable> getCollidable()
        {
            return collidables;
        }

        public void add(Controller c)
        {
            this.controllers.Add(c);
        }

        public void remove(Controller c)
        {
            this.controllers.Remove(c);
        }


        public void add(Entity e)
        {

            this.entities.Add(e);
            e.World = this;
            renderMaster.addModel(e.Model);
            if (e is Updatable)
            {
                updatables.Add(e as Updatable);
            }
            if (e is Collidable)
            {
                collidables.Add(e as Collidable);
            }

        }

        public void remove(Entity e)
        {
            this.entities.Remove(e);
            e.World = null;
            renderMaster.removeModel(e.Model);
            if (e is Updatable)
            {
                updatables.Remove(e as Updatable);
            }
            if (e is Collidable)
            {
                collidables.Remove(e as Collidable);
            }
        }

    }
}
