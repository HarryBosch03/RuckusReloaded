using System;
using System.Collections.Generic;
using UnityEngine;

namespace RuckusReloaded.Runtime.Npc.StateMachines
{
    public class StateMachine<T> : State<T>
    {
        public readonly List<State<T>> states = new();
        public readonly Blackboard blackboard;

        private int currentStateIndex;
        private T target;

        public override T Target => StateMachine != null ? StateMachine.Target : target;
        public State<T> CurrentState => states[currentStateIndex];

        public StateMachine(string name, Blackboard blackboard) : base(name) { this.blackboard = blackboard; }

        public StateMachine(string name, T target) : base(name)
        {
            this.target = target;
            blackboard = new Blackboard();
        }

        protected override void OnInitialize()
        {
            foreach (var e in states)
            {
                e.Initialize(this);
            }
        }

        public override void OnEnter()
        {
            currentStateIndex = 0;
            CurrentState?.Enter();
        }

        public override void Exit()
        {
            CurrentState?.Exit();
        }

        public override void OnFixedUpdate() { CurrentState?.FixedUpdate(); }

        public override void Update() { CurrentState?.Update(); }

        public override string ToString() { return $"[StateMachine]{name} | CurrentState:{CurrentState?.ToString() ?? "null"}"; }

        public void ChangeState(string name)
        {
            ChangeState(FindState(name));
        }

        private int FindState(string name)
        {
            for (var i = 0; i < states.Count; i++)
            {
                var state = states[i];
                if (name != state.name) continue;

                return i;
            }
            throw new NullReferenceException();
        }
        
        public void AddTransition(string from, StateTransition<T> transition)
        {
            Transitions.Add(transition);
            transition.Initialize(states[FindState(from)]);
        }

        private void ChangeState(int index)
        {
            CurrentState?.Exit();
            currentStateIndex = index;
            CurrentState?.Enter();
        }
    }
}