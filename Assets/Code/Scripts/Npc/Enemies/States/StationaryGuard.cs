using System;
using RuckusReloaded.Runtime.Npc.StateMachines;
using RuckusReloaded.Runtime.Player;
using UnityEngine;

namespace RuckusReloaded.Runtime.Npc.Enemies.States
{
    [Serializable]
    public class StationaryGuard : State<BipedalNpc>
    {
        public float range = 30.0f;
        
        public Transition next;
        private Vector3 position;

        public override void Enter()
        {
            position = Target.transform.position;
        }

        public override void FixedUpdate()
        {
            Target.PathTo(position);

            foreach (var e in PlayerController.All)
            {
                var dist = (e.transform.position - Target.transform.position).magnitude;
                if (dist > range) continue;
                
                Blackboard.Set("agro", e.gameObject);
                sm.ChangeState(next());
                break;
            }
        }
    }
}