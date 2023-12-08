namespace RuckusReloaded.Runtime.Npc.StateMachines
{
    public abstract class StateTransition<T>
    {
        private State<T> from;
        private string to;

        protected State<T> State => from;
        
        protected StateTransition(string to)
        {
            this.to = to;
        }

        public void Initialize(State<T> from)
        {
            this.from = from;
            OnInitialize();
        }

        protected virtual void OnInitialize() { }

        public bool Transition()
        {
            var res = DoTransition();
            if (res)
            {
                from.StateMachine.ChangeState(to);
            }
            return res;
        }
        
        protected abstract bool DoTransition();
    }
}