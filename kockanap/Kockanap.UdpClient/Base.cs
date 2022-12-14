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

        public List<Vector2> points { get; set; }

        public Base(int id)
        {
            this.Id = id;
            points = new List<Vector2>();
        }

        public Base(int id, int x, int y)
        {
            this.Id = id;
            this.BasePoint = new Vector2(x, y);
            points = new List<Vector2>();
        }

        public void CalculateCenterPoint()
        {
            Vector2 sumVec = new Vector2(0,0);
            foreach (var item in points)
            {
                sumVec += item;
            }

            BasePoint = sumVec / points.Count;
            var floorX = (int)Math.Floor(BasePoint.X);
            var floorY = (int)Math.Floor(BasePoint.Y);
            BasePoint = new Vector2(floorX, floorY);
        }
    }
}
