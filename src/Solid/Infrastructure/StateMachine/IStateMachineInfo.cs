//----------------------------------------------------------------------------------
// <copyright file="IStateMachineInfo.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2018. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Solid.Infrastructure.StateMachine
{
    public interface IStateMachineInfo
    {
        /// <summary>
        /// Exposes the states, transitions, and actions of this machine.
        /// </summary>
        IEnumerable<IStateInfo> States { get; }

        /// <summary>
        /// The type of the underlying state.
        /// </summary>
        /// <returns></returns>
        Type StateType { get; }

        /// <summary>
        /// The type of the underlying trigger.
        /// </summary>
        /// <returns></returns>
        Type TriggerType { get; }
    }

    public interface IStateInfo
    {
        ///// <summary>
        ///// The instance or value this state represents.
        ///// </summary>
        //object UnderlyingState { get; }

        /// <summary>
        /// Substates defined for this StateResource.
        /// </summary>
        IEnumerable<IStateInfo> Substates { get; }

        /// <summary>
        /// Superstate defined, if any, for this StateResource.
        /// </summary>
        IStateInfo Superstate { get; }

        /// <summary>
        /// Actions that are defined to be exectuted on state-entry.
        /// </summary>
        IEnumerable<string> EntryActions { get; }

        /// <summary>
        /// Actions that are defined to be exectuted on state-exit.
        /// </summary>
        IEnumerable<string> ExitActions { get; }

        /// <summary> 
        /// Transitions defined for this state. 
        /// </summary> 
        IEnumerable<ITransitionInfo> Transitions { get; }
        /// <summary>
        /// Transitions defined for this state.
        /// </summary>
        IEnumerable<IFixedTransitionInfo> FixedTransitions { get; }

        /// <summary>
        /// Dynamic Transitions defined for this state internally.
        /// </summary>
        IEnumerable<IDynamicTransitionInfo> DynamicTransitions { get; }

        /// <summary>
        /// Triggers ignored for this state.
        /// </summary>
        IEnumerable<ITriggerInfo> IgnoredTriggers { get; }

        /// <summary>
        /// Passes through to the value's ToString.
        /// </summary>
        string ToString();
    }

    /// <summary>
    /// ITransitionInfo
    /// </summary>
    public interface ITransitionInfo
    {
        /// <summary>
        /// The trigger whose firing resulted in this transition.
        /// </summary>
        ITriggerInfo Trigger { get; }

        /// <summary>
        /// Description of provided guard clause, if any.
        /// </summary>
        string GuardDescription { get; }
    }

    /// <summary>
    /// Describes a transition that can be initiated from a trigger.
    /// </summary>
    public interface IFixedTransitionInfo : ITransitionInfo
    {
        /// <summary>
        /// The state that will be transitioned into on activation.
        /// </summary>
        IStateInfo DestinationState { get; }
    }

    /// <summary>
    /// Describes a transition that can be initiated from a trigger, but whose result is non-deterministic.
    /// </summary>
    public interface IDynamicTransitionInfo : ITransitionInfo
    {
        /// <summary>
        /// Friendly text for dynamic transitions.
        /// </summary>
        string Destination { get; }
    }

    /// <summary>
    /// Represents a trigger in a state machine.
    /// </summary>
    public interface ITriggerInfo
    {
        /// <summary>
        /// The instance or value this trigger represents.
        /// </summary>
        object UnderlyingTrigger { get; }

        /// <summary>
        /// Describes the trigger.
        /// </summary>
        string ToString();
    }

}
