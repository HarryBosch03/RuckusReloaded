using RuckusReloaded.Runtime.Npc.StateMachines;
using UnityEngine;

namespace RuckusReloaded.Runtime.Npc
{
    public abstract class NpcBehaviour : MonoBehaviour
    {
        public string currentState;
        
        private StateMachine<NpcBehaviour> stateMachine;
        
        public abstract void BuildStateMachine(StateMachine<NpcBehaviour> sm);

        private void Awake()
        {
            stateMachine = new StateMachine<NpcBehaviour>(null, this);
            BuildStateMachine(stateMachine);
            stateMachine.Initialize(null);
        }

        private void OnEnable()
        {
            stateMachine.Enter();
        }

        private void FixedUpdate()
        {
            stateMachine.FixedUpdate();

            currentState = stateMachine.CurrentState.name ?? "null";
        }

        private void Update()
        {
            stateMachine.Update();
        }

        private void OnDisable()
        {
            stateMachine.Exit();
        }
    }
}