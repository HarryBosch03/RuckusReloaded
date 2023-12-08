using RuckusReloaded.Runtime.Npc.StateMachines;
using UnityEngine;

namespace RuckusReloaded.Runtime.Npc
{
    public abstract class NpcBehaviour<T> : MonoBehaviour
    {
        public string currentState;
        
        private StateMachine<T> stateMachine;
        
        public abstract State<T> MakeTree();

        private void Awake()
        {
            var target = GetComponent<T>();
            
            stateMachine = new StateMachine<T>(target);
            stateMachine.ChangeState(MakeTree());
        }
        
        private void FixedUpdate()
        {
            stateMachine.FixedUpdate();

            currentState = stateMachine.state?.GetType().Name ?? "null";
        }

        private void Update()
        {
            stateMachine.Update();
        }
    }
}