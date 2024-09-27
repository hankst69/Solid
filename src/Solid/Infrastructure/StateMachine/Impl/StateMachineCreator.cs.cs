//----------------------------------------------------------------------------------
// File: "IStateMachineCreator.cs"
// Author: Steffen Hanke
// Date: 2023
//----------------------------------------------------------------------------------
using Solid.Infrastructure.Diagnostics;
using System;

namespace Solid.Infrastructure.StateMachine.Impl
{
    public class StateMachineCreator : IStateMachineCreator
    {
        private readonly ITracer _tracer = null;

        public StateMachineCreator() {}

        public StateMachineCreator(ITracer tracer)
        {
            ConsistencyCheck.EnsureArgument(tracer).IsNotNull();
            _tracer = tracer;
        }

        public IStateMachine<TState, TTrigger> Create<TState, TTrigger>()
            where TState : struct, IConvertible, IComparable, IFormattable
            where TTrigger : struct, IConvertible, IComparable, IFormattable 
            => new StateMachine<TState, TTrigger>(_tracer);
    }
}
