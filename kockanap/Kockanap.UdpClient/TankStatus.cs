using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kockanap.Client
{
    internal class TankStatus
    {
        public enum State
        {
            Charging, BackToBase, Attacking, Flee, Exploring, FindNewTarget
        }

        public TankStatus()
        {
            ResetTank();
        }

        public bool EngineOn { get; set; }
        public bool CannonOn { get; set; }
        public int Rotation { get; set; }
        public bool HoldingGate { get; set; }

        public State AIState { get; set; }

        public void ResetTank()
        {
            EngineOn = false;
            CannonOn = false;
            Rotation = 0;
            HoldingGate = false;
            AIState = State.FindNewTarget;
        }

        public override string ToString()
        {
            return String.Format("EngineOn: {0}, CannonOn: {1}, Rotation: {2}, HoldingGate: {3}, State: {4}", EngineOn, CannonOn, Rotation, HoldingGate, AIState.ToString());
        }
    }
}
