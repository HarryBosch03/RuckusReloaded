using RuckusReloaded.Runtime.Npc.StateMachines;

namespace RuckusReloaded.Runtime.Npc.Enemies.States
{
    public class StandIdle : State<NpcBehaviour>
    {
        private BipedalNpc movement;

        public StandIdle(string name) : base(name) { }
        
        public override void OnEnter()
        {
            movement = Target.GetComponent<BipedalNpc>();
        }

        public override void OnFixedUpdate()
        {
            movement.PathTo(Target.transform.position);
        }
    }
}