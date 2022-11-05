using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Kockanap.Client
{
    internal class Base
    {
        public int Id { get; set; }
        public Vector2 BasePoint { get; set; }

        public Base(int id)
        {
            this.Id = id;
        }

        public void CalculateCenterPoint(List<Vector2> list)
        {
            Vector2 sumVec = new Vector2(0,0);
            foreach (var item in list)
            {
                sumVec += item;
            }

            BasePoint = sumVec / list.Count;
        }
    }
}
