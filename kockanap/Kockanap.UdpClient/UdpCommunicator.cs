using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Kockanap.Client
{
    internal class UdpCommunicator
    {
        string remoteIp;
        string hostIp;
        int hostPort;
        int remotePort;
        UdpClient senderClient;
        UdpClient receiverClient;
        IPEndPoint remoteEndPoint;
        public List<TankInfo> tankCache;
        public MapInfo mapCache;
        public UdpCommunicator(string remoteIp, int remotePort, int hostPort)
        {
            this.remoteIp = remoteIp;
            this.remotePort = remotePort;
            this.hostPort = hostPort;
            senderClient = new UdpClient(10000);
            receiverClient = new UdpClient(this.hostPort);
            this.remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
        }

        public void Start()
        {
            try
            {
                ConnectionSetup();
            }
            catch
            {
                Console.WriteLine("Connection error");
            }
        }

        private void ConnectionSetup()
        {
            senderClient.Connect("10.8.11.150", remotePort);
            receiverClient.Connect("10.8.11.150", 0);
        }

        public void Send(string data)
        {
            Byte[] sendBytes = Encoding.ASCII.GetBytes(data);
            senderClient.Send(sendBytes, sendBytes.Length);
        }

        public int CheckContentType(byte[] received)
        {
            if (received == null) return -1;
            else if (received[0] == 127 && received[1] == 255 && received[2] == 127 && received[3] == 255)
            {
                return 1;
            }
            return 2;
        }

        public void FillDataCache()
        {
            Byte[] receiveBytes = receiverClient.Receive(ref this.remoteEndPoint);
            switch (CheckContentType(receiveBytes))
            {
                case -1:
                    break;
                case 1:
                    GetTankInfos(receiveBytes);
                    break;
                case 2:
                    GetMapInfo(receiveBytes);
                    break;
                default:
                    break;
            }
        }

        public void GetMapInfo(byte[] receiveBytes)
        {
            if(this.mapCache == null)
            {
                this.mapCache = new MapInfo(receiveBytes);
            }
            else
            {
                this.mapCache.data = receiveBytes;
            }
        }

        public void GetTankInfos(byte[] receiveBytes)
        {
            List<TankInfo> tankInfos = new List<TankInfo>();
            int numberOfTanks = (int)receiveBytes[4] * (int)256 + (int)receiveBytes[5];
            for (int i = 0; i < numberOfTanks; i++)
            {
                try
                {
                    var playerid = receiveBytes[6 + i * 12];
                    var tankid = receiveBytes[7 + i * 12];
                    var campX = receiveBytes[8 + i * 12] * 256 + receiveBytes[9 + i * 12];
                    var campY = receiveBytes[10 + i * 12] * 256 + receiveBytes[11 + i * 12];
                    var X = receiveBytes[12 + i * 12] * 256 + receiveBytes[13 + i * 12];
                    var Y = receiveBytes[14 + i * 12] * 256 + receiveBytes[15 + i * 12];
                    var energy = receiveBytes[16 + i * 12];
                    var shield = receiveBytes[17 + i * 12];
                    tankInfos.Add(new TankInfo(playerid, tankid, campX, campY, X, Y, energy, shield));
                }
                catch
                {
                    Console.WriteLine("Random error ami nem érdekel senkit a világon");
                }
            }

            this.tankCache = tankInfos;
        }
    }
}
