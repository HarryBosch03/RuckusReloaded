using System.Collections.Generic;

namespace RuckusReloaded.Runtime.Npc.StateMachines
{
    public abstract class State<T>
    {
        public StateMachine<T> sm;

        public T Target => sm.target;
        public Blackboard Blackboard => sm.blackboard;
        
        public virtual void Enter() { }
        public virtual void FixedUpdate() { }
        public virtual void Update() { }
        public virtual void Exit() { }

        public delegate State<T> Transition();
    }
    
}