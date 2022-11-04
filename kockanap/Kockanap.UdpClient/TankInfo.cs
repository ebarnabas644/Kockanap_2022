using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kockanap.Client
{
    internal struct TankInfoStruct
    {
        public byte PlayerId { get; set; }
        public byte tankId { get; set; }
        public ushort campX { get; set; }
        public ushort campY { get; set; }
        public ushort X { get; set; }
        public ushort Y { get; set; }
        public byte energy { get; set; }
        public byte shield { get; set; }

        public override string ToString()
        {
            return String.Format("PlayerId: {0}, tankId: {1}, campX: {2}, campY: {3}, X: {4}, Y: {5}, energy: {6}, shield: {7}", PlayerId, tankId, campX, campY, X, Y, energy, shield);
        }
    }

    internal class TankInfo
    {
        public TankInfo(TankInfoStruct tankInfoStruct)
        {
            this.PlayerId = tankInfoStruct.PlayerId;
            this.tankId = tankInfoStruct.tankId;
            this.campX = (int)tankInfoStruct.campX >> 8;
            this.campY = (int)tankInfoStruct.campY >> 8;
            this.X = (int)tankInfoStruct.X >> 8;
            this.Y = (int)tankInfoStruct.Y >> 8;
            this.energy = tankInfoStruct.energy;
            this.shield = tankInfoStruct.shield;
        }

        public TankInfo(byte playerId, byte tankId, int campX, int campY, int x, int y, byte energy, byte shield)
        {
            PlayerId = playerId;
            this.tankId = tankId;
            this.campX = campX;
            this.campY = campY;
            X = x;
            Y = y;
            this.energy = energy;
            this.shield = shield;
        }

        public byte PlayerId { get; set; }
        public byte tankId { get; set; }
        public int campX { get; set; }
        public int campY { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public byte energy { get; set; }
        public byte shield { get; set; }



        public override string ToString()
        {
            return String.Format("PlayerId: {0}, tankId: {1}, campX: {2}, campY: {3}, X: {4}, Y: {5}, energy: {6}, shield: {7}", PlayerId, tankId, campX, campY, X, Y, energy, shield);
        }
    }
}
