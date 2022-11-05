using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Kockanap.Client
{
    internal class MapInfo
    {
        public byte[] data { get; set; }

        public List<Gate> Stargates { get; set; }

        public List<Base> Bases { get; set; }

        public Base Default { get; set; }

        public MapInfo(byte[] data)
        {
            this.data = data;
            Stargates = new List<Gate>();
            Bases = new List<Base>();
        }

        public void ResetMapInfo()
        {
            Stargates.Clear();
            Bases.Clear();

        }

        public Base NearestBase(Vector2 currentPos)
        {
            if (Bases.Count == 0) return Default;
            if (Bases.Count == 1)
            {
                if(currentPos.X < 0 || currentPos.X > 700 && currentPos.Y < 0 || currentPos.Y > 700)
                {
                    return Default;
                }
                return Bases[0];
            }

            double maxDistance = double.MaxValue;
            int idx = 0;
            for (int i = 0; i < Bases.Count; i++)
            {
                double d = Math.Sqrt(Math.Pow(Bases[i].BasePoint.X - currentPos.X, 2) + Math.Pow(Bases[i].BasePoint.Y - currentPos.Y, 2));
                if (maxDistance > d)
                {
                    maxDistance = d;
                    idx = i;
                }
            }

            if (currentPos.X < 0 || currentPos.X > 700 && currentPos.Y < 0 || currentPos.Y > 700)
            {
                return Default;
            }
            return Bases[idx];
        }

        public Gate NearestStargate(Vector2 currentPos)
        {
            if(Stargates.Count == 0)
            {
                return null;
            }
            if (Stargates.Count == 1)
            {
                return Stargates[0];
            }

            double maxDistance = double.MaxValue;
            int idx = 0;
            for (int i = 0; i < Stargates.Count; i++)
            {
                double d = Math.Sqrt(Math.Pow(Stargates[i].Pos.X - currentPos.X, 2) + Math.Pow(Stargates[i].Pos.Y - currentPos.Y, 2));
                if (maxDistance > d)
                {
                    maxDistance = d;
                    idx = i;
                }
            }

            return Stargates[idx];
        }

        //public void AddBase(int id, List<Vector2> pos)
        //{

        //    foreach (var item in Bases)
        //    {
        //        if(item.Id == id)
        //        {
        //            item.CalculateCenterPoint(pos);
        //            return;
        //        }
        //    }
        //    var newBase = new Base(id);
        //    Bases.Add(newBase);
        //    newBase.CalculateCenterPoint(pos);
        //}

        public void AddStargate(Vector2 pos)
        {
            foreach (var item in Stargates)
            {
                if(Vector2.DistanceSquared(item.Pos, pos) < Math.Pow(10, 2))
                {
                    return;
                }
            }
            Stargates.Add(new Gate(pos));
        }
    }
}
