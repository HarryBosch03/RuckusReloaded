using UnityEngine;

namespace RuckusReloaded.Runtime.Npc.StateMachines
{
    public class StateMachine<T>
    {
        public State<T> state;
        public readonly T target;
        public readonly Blackboard blackboard;
        
        public StateMachine(T target)
        {
            this.target = target;
            blackboard = new Blackboard();
        }
        
        public void FixedUpdate()
        {
            state?.FixedUpdate();
        }

        public void Update()
        {
            state?.Update();
        }

        public void ChangeState(State<T> newState)
        {
            Debug.Log($"Changed State from {state} to {newState}");
            
            state?.Exit();
            state = newState;
            state.sm = this;
            state?.Enter();
        }
    }
}