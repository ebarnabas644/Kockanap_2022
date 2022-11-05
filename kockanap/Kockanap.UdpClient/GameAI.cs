using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static Kockanap.Client.TankStatus;
using System.Numerics;

namespace Kockanap.Client
{
    internal class GameAI
    {
        public enum TargetType
        {
            Land, Base, Enemy, Stargate
        }


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
        private const int borderSize = 20;
        private Random rnd = new Random();
        private Base nearestBase;
        private int rotationFiness = 5;
        private int previousEnergy;
        private int chargingCounter = 0;
        private int chargingingFailed = 0;
        private List<TankInfo> detectedEnemies;
        private TargetType targetType;

        public GameAI(int tankId)
        {
            udpCommunicator = new UdpCommunicator("10.8.11.150", 5555, 11000);
            mapInfo = new MapInfo(new byte[0]);
            tankInfos = new List<TankInfo>();
            counter = 0;
            tankStatus = new TankStatus();
            TankId = tankId;
            nearestBase = new Base(-1);
            detectedEnemies = new List<TankInfo>();
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
            if(counter % 50 == 0)
            {
                previousEnergy = controlledTank.energy;
            }
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
                case State.Attack:

                default:
                    Exploring();
                    break;
            }
            CheckForReset();
        }

        private void Exploring()
        {
            chargingingFailed = 0;
            StartEngine();
            if(controlledTank.energy <= 0)
            {
                tankStatus.AIState = State.BackToBase;
            }
            if (TargetReached())
            {
                if (!tankStatus.HoldingGate)
                {
                    PickupGate();
                }
                tankStatus.AIState = State.FindNewTarget;
            }

            ScanForBase();
            ScanForStargate();
            if(detectedEnemies.Count > 0)
            {
                tankStatus.AIState = State.Attack;
            }
        }

        private void Attack()
        {

        }

        private void ScanForEnemy()
        {
            detectedEnemies.Clear();
            if (mapInfo == null) return;
            for (int y = 0; y < viewW; y++)
            {
                for (int x = 0; x < viewW; x++)
                {
                    byte cell = getXY(x, y);
                    if(cell != TankId && cell > 100 && cell < 200)
                    {
                        if(x > 0 && y > 0 && y < viewW - 1 && x < viewW - 1)
                        {
                            if(!(getXY(x + 1,y) < 100 && getXY(x - 1, y) < 100 && getXY(x, y + 1) < 100 && getXY(x, y-1) < 100))
                            {
                                foreach (var item in tankInfos)
                                {
                                    if (cell == item.tankId)
                                    {
                                        detectedEnemies.Add(item);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }

        }
        }

        private void ScanForStargate()
        {

            Vector2 localPlayerPos = new Vector2();
            for (int y = 0; y < viewW; y++)
            {
                for (int x = 0; x < viewW; x++)
                {
                    byte cell = getXY(x, y);
                    if (cell == 240)
                    {
                        localPlayerPos.X = x;
                        localPlayerPos.Y = y;
                        Vector2 stargatePos = new Vector2(x, y) - localPlayerPos + new Vector2(controlledTank.X, controlledTank.Y);

                        if (!mapInfo.Stargates.Contains(stargatePos))
                        {
                            mapInfo.AddStargate(new Vector2(x, y) - localPlayerPos + new Vector2(controlledTank.X, controlledTank.Y));
                        }
                        
                    }
                }
                }
            }

        private void ScanForBase()
        {
            Dictionary<int, List<Vector2>> tempBases = new Dictionary<int, List<Vector2>>();
            if (mapInfo == null) return;
            Vector2 localPlayerPos = new Vector2();
            bool alreadyFound = false;
            for (int y = 0; y < viewW; y++)
            {
                for (int x = 0; x < viewW; x++)
                {
                    byte cell = getXY(x, y);
                    if(cell == TankId)
                    {
                        localPlayerPos.X = x;
                        localPlayerPos.Y = y;
                        alreadyFound = true;
                        break;
                    }
                }
                if (alreadyFound)
                {
                    break;
                }
            }

            var contains = false;

            for (int y = 0; y < viewW; y++)
            {
                for (int x = 0; x < viewW; x++)
                {
                    byte cell = getXY(x, y);
                    if (cell < 100 && cell > 0)
                    {
                        foreach (var item in mapInfo.Bases)
                        {
                            if(item.Id == cell)
                            {
                                item.points.Add(new Vector2(x, y) - localPlayerPos + new Vector2(controlledTank.X, controlledTank.Y));
                                contains = true;
                                break;
                            }
                        }
                        if (!contains)
                        {
                            Base newBase = new Base(cell);
                            newBase.points.Add(new Vector2(x, y));
                            mapInfo.Bases.Add(newBase);
                        }
                        contains = false;
                        //if (!tempBases.ContainsKey(cell))
                        //{
                        //    tempBases.Add(cell, new List<Vector2>());
                        //    tempBases[cell].Add(new Vector2(x, y) - localPlayerPos + new Vector2(controlledTank.X, controlledTank.Y));
                        //}
                        //else
                        //{
                        //    tempBases[cell].Add(new Vector2(x, y) - localPlayerPos + new Vector2(controlledTank.X, controlledTank.Y));
                        //}
                    }
                }
            }
            foreach (var item in mapInfo.Bases)
            {
                item.CalculateCenterPoint();
            }
            
        }

        private void SetExplorationTarget()
        {
            if (!tankStatus.HoldingGate)
            {
                if(mapInfo != null)
                {
                    Vector2 stargateLoc = mapInfo.NearestStargate(new Vector2(controlledTank.X, controlledTank.Y));
                    double length = Math.Sqrt(Math.Pow(stargateLoc.X - controlledTank.X, 2) + Math.Pow(stargateLoc.Y - controlledTank.Y, 2));
                    if (length < 200.0f)
                    {
                        target = stargateLoc;
                        targetType = TargetType.Stargate;
                        return;
                    }
                }
            }
            

            target = new Vector2(rnd.Next(mapHeight - borderSize * 2) + borderSize, rnd.Next(mapWidth - borderSize * 2) + borderSize);
            targetType = TargetType.Land;

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
                if(counter % 200 == 0)
                {
                    if (previousEnergy == controlledTank.energy)
                    {
                        chargingingFailed++;
                        tankStatus.AIState = State.BackToBase;
                    }
                    else
                    {
                        if(chargingingFailed > 0)
                        {
                            chargingingFailed--;
                        }
                    }
                    if (chargingingFailed >= 2)
                    {
                        mapInfo.Bases.Remove(nearestBase);
                        chargingingFailed = 0;
                    }
                }
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
            nearestBase = mapInfo.NearestBase(new Vector2(controlledTank.X, controlledTank.Y));
            RotateToTarget(nearestBase.BasePoint);
            targetType = TargetType.Base;
            StartEngine();
            if (CurrentlyOnBase())
            {
                tankStatus.AIState = State.Charging;
            }
            if (tankStatus.HoldingGate)
            {
                DropGate();
            }
        }

        private bool CurrentlyOnBase()
        {
            if(nearestBase.BasePoint.X - 3 < controlledTank.X &&
                nearestBase.BasePoint.X + 3 > controlledTank.X &&
                nearestBase.BasePoint.Y - 3 < controlledTank.Y &&
               nearestBase.BasePoint.Y + 3 > controlledTank.Y)
            {
                return true;
            }
            else
            {
                return false;
            }
            //return nearestBase.BasePoint.X - 1 < controlledTank.X &&
            //    nearestBase.BasePoint.X + 1 > controlledTank.X &&
            //    nearestBase.BasePoint.Y - 1 < controlledTank.Y &&
            //   nearestBase.BasePoint.Y + 1 > controlledTank.Y;
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
                mapInfo?.ResetMapInfo();
                FindNewTarget();
            }
        }

        private void LogState()
        {
            if(counter % 100 == 0)
            {
                Console.Clear();
            }
            LogNearestBase();
            Console.WriteLine(tankStatus);
            Console.WriteLine("Previous energy: " + previousEnergy + ", charging failed: "+chargingingFailed);
            Console.WriteLine("Detected enemies: ");
            LogDetectedEnemies();
            Console.WriteLine("Detected stargates:");
            LogStargates();
            LogTargetPoint();
            Console.WriteLine("-------------------------------");
            LogTanks();
            Console.WriteLine("-------------------------------");
            Console.SetCursorPosition(30, 0);
            
            DrawGameMap();
            Console.SetCursorPosition(0, 0);
        }

        private void LogStargates()
        {
            if(mapInfo != null)
            {
                foreach (var item in mapInfo.Stargates)
                {
                    Console.WriteLine(String.Format("X: {0}, Y: {1}", item.X, item.Y));
                }
            }
        }

        private void LogNearestBase()
        {
            Console.WriteLine(String.Format("Nearest base: Id: {0}, X: {1}, Y: {2}", nearestBase.Id, nearestBase.BasePoint.X, nearestBase.BasePoint.Y));
        }

        private void LogTanks()
        {
            foreach (var item in tankInfos)
            {
                Console.WriteLine(item + "                  ");
            }
        }

        private void LogDetectedEnemies()
        {
            foreach (var item in detectedEnemies)
            {
                Console.Write(item.tankId +", ");
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
                        line += (char)(cell+50)+" ";
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
        private void RotateToTarget(Vector2 target)
        {
            int rot = (int)Math.Round(Math.Atan2(target.Y - controlledTank.Y, target.X - controlledTank.X) / Math.PI * 180, 0);
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
