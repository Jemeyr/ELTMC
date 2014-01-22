using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomModelEffect.Control
{
    abstract class Controller
    {

        public abstract void update();

        public enum Direction{Up, Down, Left, Right};

        public abstract bool jump();


        public abstract bool move(Direction d);

        public abstract bool look(Direction d);

        public abstract bool shoot();

        public abstract bool debug();
    }
}
