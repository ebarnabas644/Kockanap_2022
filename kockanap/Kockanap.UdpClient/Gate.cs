using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Kockanap.Client
{
    internal class Gate
    {
        public Vector2 Pos { get; set; }
        public bool isOnBase { get; set; }
        public bool currentlyHold { get; set; }

        public Gate(Vector2 pos)
        {
            Pos = pos;
            this.isOnBase = false;
            this.currentlyHold = false;
        }
    }
}
