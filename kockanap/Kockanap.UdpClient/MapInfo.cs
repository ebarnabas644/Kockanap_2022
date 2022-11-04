using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kockanap.Client
{
    internal class MapInfo
    {
        public byte[] data { get; set; }

        public MapInfo(byte[] data)
        {
            this.data = data;
        }
    }
}
