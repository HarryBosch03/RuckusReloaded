using System;

namespace RuckusReloaded.Runtime.Npc.StateMachines
{
    public class InlineTransition<T> : StateTransition<T>
    {
        private readonly Func<bool> callback;

        public InlineTransition(string to, Func<bool> callback) : base(to)
        {
            this.callback = callback;
        }
        
        protected override bool DoTransition() => callback?.Invoke() ?? false;
    }
}