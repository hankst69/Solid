//----------------------------------------------------------------------------------
// <copyright file="StateMachineInfo.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2018. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace Solid.Infrastructure.StateMachine.Impl
{
    public class StateMachineInfo : IStateMachineInfo
    {
        internal StateMachineInfo(IEnumerable<IStateInfo> states, Type stateType, Type triggerType)
        {
            if (states == null)
                throw new ArgumentNullException("states");

            States = states as IStateInfo[] ?? states.ToArray();
            StateType = stateType;
            TriggerType = triggerType;
        }

        public IEnumerable<IStateInfo> States { get; private set; }
        public Type StateType { get; private set; }
        public Type TriggerType { get; private set; }
    }

    public class StateInfo : IStateInfo
    {
        internal StateInfo(
            object underlyingState,
            IEnumerable<string> entryActions,
            IEnumerable<string> exitActions,
            IEnumerable<ITriggerInfo> ignoredTriggers)
        {
            if (underlyingState == null) throw new ArgumentNullException("underlyingState");
            if (entryActions == null) throw new ArgumentNullException("entryActions");
            if (exitActions == null) throw new ArgumentNullException("exitActions");
            if (ignoredTriggers == null) throw new ArgumentNullException("ignoredTriggers");

            UnderlyingState = underlyingState;
            EntryActions = entryActions;
            ExitActions = exitActions;
            IgnoredTriggers = ignoredTriggers;
        }

        internal void AddRelationships(
            IStateInfo superstate,
            IEnumerable<IStateInfo> substates,
            IEnumerable<IFixedTransitionInfo> transitions,
            IEnumerable<IDynamicTransitionInfo> dynamicTransitions)
        {
            //if (superstate == null) throw new ArgumentNullException("superstate"); //null is allowed
            if (substates == null) throw new ArgumentNullException("substates");
            if (transitions == null) throw new ArgumentNullException("transitions");
            if (dynamicTransitions == null) throw new ArgumentNullException("dynamicTransitions");

            Superstate = superstate;
            Substates = substates;
            FixedTransitions = transitions;
            DynamicTransitions = dynamicTransitions;
        }

        public object UnderlyingState { get; private set; }
        public IEnumerable<IStateInfo> Substates { get; private set; }
        public IStateInfo Superstate { get; private set; }
        public IEnumerable<string> EntryActions { get; private set; }
        public IEnumerable<string> ExitActions { get; private set; }
        public IEnumerable<ITransitionInfo> Transitions { get { return FixedTransitions.Concat<ITransitionInfo>(DynamicTransitions); } }
        public IEnumerable<IFixedTransitionInfo> FixedTransitions { get; private set; }
        public IEnumerable<IDynamicTransitionInfo> DynamicTransitions { get; private set; }
        public IEnumerable<ITriggerInfo> IgnoredTriggers { get; private set; }
        public override string ToString()
        {
            return UnderlyingState != null ? UnderlyingState.ToString() : "<null>";
        }
    }

    public abstract class TransitionInfo : ITransitionInfo
    {
        public ITriggerInfo Trigger { get; protected set; }
        public string GuardDescription { get; protected set; }
    }

    public class FixedTransitionInfo : TransitionInfo, IFixedTransitionInfo
    {
        internal FixedTransitionInfo(ITriggerInfo triggerInfo, string guardDescription, IStateInfo destinationStateInfo)
        {
            Trigger = triggerInfo;
            GuardDescription = string.IsNullOrWhiteSpace(guardDescription) ? null : guardDescription;
            DestinationState = destinationStateInfo;
        }

        public IStateInfo DestinationState { get; private set; }
    }

    public class DynamicTransitionInfo : TransitionInfo, IDynamicTransitionInfo
    {
        internal DynamicTransitionInfo(ITriggerInfo triggerInfo, string guardDescription, string destinationStateInfo)
        {
            Trigger = triggerInfo;
            GuardDescription = string.IsNullOrWhiteSpace(guardDescription) ? null : guardDescription;
            Destination = destinationStateInfo;
        }

        public string Destination { get; private set;}
    }

    public class TriggerInfo : ITriggerInfo
    {
        internal TriggerInfo(object underlyingTrigger)
        {
            UnderlyingTrigger = underlyingTrigger;
        }

        public object UnderlyingTrigger { get; private set; }
        public override string ToString()
        {
            return UnderlyingTrigger != null ? UnderlyingTrigger.ToString() : "<null>";
        }
    }

}
