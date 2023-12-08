using RuckusReloaded.Runtime.Npc.Enemies.States;
using RuckusReloaded.Runtime.Npc.StateMachines;

namespace RuckusReloaded.Runtime.Npc.Enemies.Behaviours
{
    public class Brian : NpcBehaviour
    {
        public override void BuildStateMachine(StateMachine<NpcBehaviour> sm)
        {
            sm.states.Add(new StandIdle("Idle"));
        }
    }
}