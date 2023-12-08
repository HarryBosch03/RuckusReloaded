using RuckusReloaded.Runtime.Npc.StateMachines;
using RuckusReloaded.Runtime.Player;
using UnityEngine;

namespace RuckusReloaded.Runtime.Npc.Enemies.States
{
    public class PlayerNearby : StateTransition<NpcBehaviour>
    {
        private float range;
        private float angle;

        private BipedalNpc movement;

        public PlayerNearby(string to, float range, float angle) : base(to)
        {
            this.range = range;
            this.angle = angle;
        }

        protected override void OnInitialize() { movement = State.Target.GetComponent<BipedalNpc>(); }

        protected override bool DoTransition()
        {
            var agro = State.StateMachine.blackboard.Get<GameObject>("agro");
            if (agro) return true;
            
            var view = movement.Movement.view;

            foreach (var e in PlayerController.All)
            {
                var diff = e.Biped.Center - view.position;
                if (diff.magnitude > range) continue;
                if (Mathf.Acos(Vector3.Dot(diff.normalized, view.forward)) > angle * Mathf.Deg2Rad) continue;

                agro = e.gameObject;
                break;
            }

            State.StateMachine.blackboard.Set("agro", agro);
            return agro;
        }
    }
}