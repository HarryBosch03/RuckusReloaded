using System;

namespace RuckusReloaded.Runtime.Npc.StateMachines
{
    public class InlineState<T> : State<T>
    {
        private Action<InlineState<T>> enterCallback;
        private Action<InlineState<T>> exitCallback;
        private Action<InlineState<T>> updateCallback;
        private Action<InlineState<T>> fixedUpdateCallback;

        public InlineState(string name) : base(name) { }

        public override void OnEnter() => enterCallback?.Invoke(this);
        public override void OnFixedUpdate() => fixedUpdateCallback?.Invoke(this);
        public override void Update() => updateCallback?.Invoke(this);
        public override void Exit() => exitCallback?.Invoke(this);
    }
}