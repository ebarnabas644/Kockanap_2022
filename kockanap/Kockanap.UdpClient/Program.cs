using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace Kockanap.Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            GameAI gameAI = new GameAI();
            gameAI.StartAI();
        }
    }
}