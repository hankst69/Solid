//----------------------------------------------------------------------------------
// File: "IStateMachineCreator.cs"
// Author: Steffen Hanke
// Date: 2023
//----------------------------------------------------------------------------------
using System;

namespace Solid.Infrastructure.StateMachine
{
    /// <summary>
    /// IStateMachineCreator
    /// </summary>
    public interface IStateMachineCreator
    {
        IStateMachine<TState, TTrigger> Create<TState, TTrigger>() 
            where TState : struct, IConvertible, IComparable, IFormattable
            where TTrigger : struct, IConvertible, IComparable, IFormattable;
    }
}
