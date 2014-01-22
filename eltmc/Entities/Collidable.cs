using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomModelEffect.Entities
{
    interface Collidable
    {
        CompositeModel Model
        {
            get;
            set;
        }
    }
}
