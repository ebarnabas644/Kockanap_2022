using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Kockanap.Client.TankStatus;
using System.Numerics;

namespace Kockanap.Client
{
    internal class GameAI
    {
        private UdpCommunicator udpCommunicator;
        private MapInfo mapInfo;
        private List<TankInfo> tankInfos;
        private int counter;
        private TankStatus tankStatus;
        private int TankId;
        const int sightDist = 15;
        const int viewW = sightDist * 2 + 1;
        private TankInfo controlledTank;
        private const int maxEnergy = 20;
        private Vector2 target;
        private const int mapHeight = 700;
        private const int mapWidth = 700;
        private const int borderSize = 50;
        private Random rnd = new Random();


        public GameAI(int tankId)
        {
            udpCommunicator = new UdpCommunicator("10.8.11.150", 5555, 11000);
            mapInfo = new MapInfo(new byte[0]);
            tankInfos = new List<TankInfo>();
            counter = 0;
            tankStatus = new TankStatus();
            TankId = tankId;
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
            StartEngine();
            RotateTank(120);
            while (true)
            {
                UpdateGameCache();
                GetControlledTankInfo();
                Processing();
                LogState();
                counter++;
            }
        }

        private void Processing()
        {
            switch (tankStatus.AIState)
            {
                case State.Charging:
                    Charging();
                    break;
                case State.BackToBase:
                    BackToBase();
                    break;
                case State.Attacking:
                    break;
                case State.Flee:
                    break;
                case State.Exploring:
                    Exploring();
                    break;
                case State.FindNewTarget:
                    FindNewTarget();
                    break;
                default:
                    Exploring();
                    break;
            }
            CheckForReset();
            if(controlledTank.energy <= 0)
            {
                RotateToTarget(controlledTank.campX, controlledTank.campY);
            }
        }

        private void Exploring()
        {
            StartEngine();
            if(controlledTank.energy <= 0)
            {
                tankStatus.AIState = State.BackToBase;
            }
            if (TargetReached())
            {
                tankStatus.AIState = State.FindNewTarget;
            }
            
        }

        private void SetExplorationTarget()
        {
            target = new Vector2(rnd.Next(mapHeight - borderSize * 2) + borderSize, rnd.Next(mapWidth - borderSize * 2) + borderSize);
        }

        private bool PointOutOfBounds(Vector2 point)
        {
            return point.X < borderSize ||
                point.X > mapWidth - borderSize ||
                point.Y < borderSize ||
                point.Y > mapHeight - borderSize;
        }

        private void Charging()
        {
            if(controlledTank.energy < maxEnergy - 2)
            {
                StopEngine();
                StopCannon();
            }
            else
            {
                tankStatus.AIState = State.FindNewTarget;
            }
        }

        private void FindNewTarget()
        {
            SetExplorationTarget();
            RotateToTarget((int)target.X, (int)target.Y);
            tankStatus.AIState = State.Exploring;
        }

        private bool TargetReached()
        {
            Vector2 pos = new Vector2(controlledTank.X, controlledTank.Y);
            return Vector2.DistanceSquared(pos, target) < Math.Pow(10,2);
        }

        private void BackToBase()
        {
            RotateToTarget(controlledTank.campX, controlledTank.campY);
            StartEngine();
            if (CurrentlyOnBase())
            {
                tankStatus.AIState = State.Charging;
            }
        }

        private bool CurrentlyOnBase()
        {
            return controlledTank.campX - 8 < controlledTank.X &&
                controlledTank.campX + 8 > controlledTank.X &&
                controlledTank.campY - 8 < controlledTank.Y &&
                controlledTank.campY + 8 > controlledTank.Y;
        }

        private void GetControlledTankInfo()
        {
            foreach (var item in tankInfos)
            {
                if (item.tankId == TankId)
                {
                    this.controlledTank = item;
                    return;
                }
            }
        }

        private void CheckForReset()
        {
            if(controlledTank.campX == controlledTank.X && controlledTank.campY == controlledTank.Y)
            {
                tankStatus.ResetTank();
                FindNewTarget();
            }
        }

        private void LogState()
        {
            if(counter % 100 == 0)
            {
                Console.Clear();
            }
            Console.WriteLine(tankStatus);
            LogTargetPoint();
            Console.WriteLine("-------------------------------");
            LogTanks();
            Console.WriteLine("-------------------------------");
            Console.SetCursorPosition(30, 0);
            
            DrawGameMap();
            Console.SetCursorPosition(0, 0);
        }

        private void LogTanks()
        {
            foreach (var item in tankInfos)
            {
                Console.WriteLine(item + "                  ");
            }
        }

        private void LogTargetPoint()
        {
            Console.WriteLine(String.Format("Target: X: {0}, Y: {1}", target.X, target.Y));
        }

        private void DrawGameMap()
        {
            if (mapInfo == null) return;
            for (int y = 0; y < viewW; y++)
            {
                string line = "";
                for (int x = 0; x < viewW; x++)
                {
                    byte cell = getXY(x, y);
                    if (cell == 0)
                    {
                        line += "# ";

                    }
                    else if (cell == TankId - 100)
                    {
                        line += ". ";
                    }
                    else if (cell < 100)
                    {
                        line += "_ ";
                    }
                    else if (cell == TankId)
                    {
                        line += "P ";
                    }
                    else if (cell < 200)
                    {
                        line += "E ";
                    }
                    else if (cell == 240)
                    {
                        line += "O ";
                    }
                    else if (cell == 250)
                    {
                        line += "  ";
                    }
                    else
                    {
                        line += "? ";
                    }
                }
                Console.SetCursorPosition(100, y);
                Console.Write(line);
            }
        }

        private byte getXY(int x, int y)
        {
            return mapInfo.data[y * (sightDist * 2 + 1) + x];
        }

        private void StartEngine(bool forced = false)
        {
            if(!tankStatus.EngineOn || forced)
            {
                udpCommunicator.Send("engineon");
                tankStatus.EngineOn = true;
            }
        }

        private void RotateToTarget(int targetX, int targetY)
        {
            int rot = (int)Math.Round(Math.Atan2(targetY - controlledTank.Y, targetX - controlledTank.X) / Math.PI * 180,0);
            RotateTank(rot);
        }

        private void StopEngine(bool forced = false)
        {
            if (tankStatus.EngineOn || forced)
            {
                udpCommunicator.Send("engineoff");
                tankStatus.EngineOn = false;
            }
        }

        private void StartCannon(bool forced = false)
        {
            if(!tankStatus.CannonOn || forced)
            {
                udpCommunicator.Send("cannonon");
                tankStatus.CannonOn = true;
            }
        }

        private void StopCannon(bool forced = false)
        {
            if(tankStatus.CannonOn || forced)
            {
                udpCommunicator.Send("cannonoff");
                tankStatus.CannonOn = false;
            }
        }

        private void PickupGate(bool forced = false)
        {
            if(!tankStatus.HoldingGate || forced)
            {
                udpCommunicator.Send("pickup");
                tankStatus.HoldingGate = true;
            }
        }

        private void DropGate(bool forced = false)
        {
            if(tankStatus.HoldingGate || forced)
            {
                udpCommunicator.Send("drop");
                tankStatus.HoldingGate = false;
            }
        }

        private void UseGate(int destX, int destY)
        {
            udpCommunicator.Send(String.Format("puddlejump_{0};{1}",destX, destY));
        }

        private void RotateTank(int degree, bool forced = false)
        {
            if(tankStatus.Rotation != degree || forced)
            {
                udpCommunicator.Send(String.Format("rot_{0}", degree));
                tankStatus.Rotation = degree;
            }
        }
    }
}
