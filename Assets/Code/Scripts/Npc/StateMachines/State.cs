using System.Collections.Generic;

namespace RuckusReloaded.Runtime.Npc.StateMachines
{
    public abstract class State<T>
    {
        public string name;

        public virtual T Target => StateMachine.Target;
        public StateMachine<T> StateMachine { get; private set; }
        public List<StateTransition<T>> Transitions { get; } = new();

        protected State(string name)
        {
            this.name = name;
        }

        public void Initialize(StateMachine<T> sm)
        {
            StateMachine = sm;
            OnInitialize();
        }

        protected virtual void OnInitialize() { }

        public void Enter() => OnEnter();

        public void FixedUpdate()
        {
            OnFixedUpdate();
            foreach (var e in Transitions)
            {
                if (e.Transition()) break;
            }
        }
        
        public virtual void OnEnter() { }
        
        public virtual void Update() { }
        public virtual void OnFixedUpdate() { }
        
        public virtual void Exit() { }

        public State<T> AddTransition(StateTransition<T> transition)
        {
            Transitions.Add(transition);
            transition.Initialize(this);
            return this;
        }

        public override string ToString() => $"[{GetType().Name}]{name}";
    }
}