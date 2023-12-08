using RuckusReloaded.Runtime.Npc.Enemies.States;
using RuckusReloaded.Runtime.Npc.StateMachines;

namespace RuckusReloaded.Runtime.Npc.Enemies.Behaviours
{
    public class Brian : NpcBehaviour<BipedalNpc>
    {
        public StationaryGuard idle;
        public ShootAtAgro attack;
        
        public override State<BipedalNpc> MakeTree()
        {
            idle.next = () => attack;
            attack.next = () => idle;
            
            return idle;
        }
    }
}