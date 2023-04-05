//----------------------------------------------------------------------------------
// <copyright file="IStateMachineCreator.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2023. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
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
