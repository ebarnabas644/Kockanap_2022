using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kockanap.Client
{
    internal class GameAI
    {
        private UdpCommunicator udpCommunicator;
        private MapInfo mapInfo;
        private List<TankInfo> tankInfos;
        private int counter;

        public GameAI()
        {
            udpCommunicator = new UdpCommunicator("10.8.11.150", 5555, 11000);
            mapInfo = new MapInfo(new byte[0]);
            tankInfos = new List<TankInfo>();
            counter++;
        }

        private void UpdateGameCache()
        {
            udpCommunicator.FillDataCache();
            tankInfos = udpCommunicator.tankCache;
            mapInfo = udpCommunicator.mapCache;

            //if(mapInfo != null)
            //{
            //    File.WriteAllBytes(counter +".txt", mapInfo.data);
            //}
            //counter++;

        }

        public void StartAI()
        {
            Console.WriteLine("Start AI:");
            udpCommunicator.Start();
            while (true)
            {
                UpdateGameCache();
                LogState();
            }
        }

        private void LogState()
        {
            Console.WriteLine("-------------------------------");
            LogTanks();
            Console.WriteLine("-------------------------------");
            Console.SetCursorPosition(0, 0);
        }

        private void LogTanks()
        {
            foreach (var item in tankInfos)
            {
                Console.WriteLine(item + "                  ");
            }
        }

        private void StartEngine()
        {
            udpCommunicator.Send("engineon");
        }

        private void StopEngine()
        {
            udpCommunicator.Send("engineoff");
        }

        private void StartCannon()
        {
            udpCommunicator.Send("cannonon");
        }

        private void StopCannon()
        {
            udpCommunicator.Send("cannonoff");
        }

        private void PickupGate()
        {
            udpCommunicator.Send("pickup");
        }

        private void DropGate()
        {
            udpCommunicator.Send("drop");
        }

        private void UseGate(int destX, int destY)
        {
            udpCommunicator.Send(String.Format("puddlejump_{0};{1}",destX, destY));
        }

        private void RotateTank(int degree)
        {
            udpCommunicator.Send(String.Format("rot_{0}", degree));
        }
    }
}
