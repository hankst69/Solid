//----------------------------------------------------------------------------------
// <copyright file="StateMachine.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2016-2022. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Solid.Infrastructure.Diagnostics;

namespace Solid.Infrastructure.StateMachine.Impl
{
    internal interface IStateMachineConfiguration<in TState, in TTrigger>
        where TState : struct, IConvertible, IComparable, IFormattable
        where TTrigger : struct, IConvertible, IComparable, IFormattable
    {
        void SetStartState(TState startState);

        void AddTransition(TState fromState, TTrigger trigger, TState toState, string description);
        void AddTransition(TState fromState, TTrigger trigger, TState toState, Func<bool> condition, string description);
        void AddTransition<TTriggerArg>(TState fromState, TTrigger trigger, TState toState, Func<TTriggerArg, bool> condition, string description);

        void AddEntryAction(TState fromState, Action entryAction, string description);
        void AddEntryAction<TTriggerArg>(TState fromState, Action<TTriggerArg> entryAction, string description);
        void AddEntryAction(TState fromState, TTrigger trigger, Action entryAction, string description);
        void AddEntryAction<TTriggerArg>(TState fromState, TTrigger trigger, Action<TTriggerArg> entryAction, string description);

        void AddLeaveAction(TState fromState, Action leaveAction, string description);
        void AddLeaveAction<TTriggerArg>(TState fromState, Action<TTriggerArg> leaveAction, string description);
        void AddLeaveAction(TState fromState, TTrigger trigger, Action leaveAction, string description);
        void AddLeaveAction<TTriggerArg>(TState fromState, TTrigger trigger, Action<TTriggerArg> leaveAction, string description);
    }

    public class StateMachine<TState, TTrigger> : 
        IStateMachine<TState, TTrigger>, 
        IStateMachineConfiguration<TState, TTrigger>
        where TState : struct, IConvertible, IComparable, IFormattable
        where TTrigger : struct, IConvertible, IComparable, IFormattable
    {
        private readonly IDictionary<TState, StateConfiguration> m_States = new Dictionary<TState, StateConfiguration>();
        private readonly IDictionary<Transition, ICollection<ITriggerConfiguration>> m_Transitions = new Dictionary<Transition, ICollection<ITriggerConfiguration>>();

        private readonly ITracer _tracer;

        /// <summary>
        /// Constructs a new instance of StateMachine
        /// </summary>
        /// <param name="tracer">optional interface for trace outputs</param>
        public StateMachine(ITracer tracer = null)
        {
            _tracer = tracer;
            using var ctorTrace = _tracer?.CreateScopeTracer();

            if (!typeof(TState).IsEnum)
            {
                throw new ArgumentException("TState must be an enum.");
            }
            if (!typeof(TTrigger).IsEnum)
            {
                throw new ArgumentException("TTrigger must be an enum.");
            }

            var states = Enum.GetValues(typeof (TState)).Cast<TState>();

            foreach (var state in states)
            {
                AddState(state);
            }
        }

        /// <summary>
        /// Constructs a new instance of StateMachine with given StartState
        /// </summary>
        /// <param name="startState">startState</param>
        /// <param name="tracer">optinal interface for trace outputs</param>
        public StateMachine(TState startState, ITracer tracer = null)
            : this(tracer)
        {
            SetStartState(startState);
        }

        public Action<TState, TTrigger, string> UnexpectedTriggerHandler { private get; set; }

        public Action<TState, TState, Exception, Action<TState>, Action<TState>> StateTransitionExceptionHandler { private get; set; }

        public Action<TState, TState, TTrigger, object, Type> StateTransitionCompletedHandler { private get; set; }

        public TState State { get; private set; }

        public bool IsInState(TState state)
        {
            using var tracer = _tracer?.CreateScopeTracer();

            return State.Equals(state);
        }

        public void SetStartState(TState startState)
        {
            using var tracer = _tracer?.CreateScopeTracer();

            if (!m_States.ContainsKey(startState))
            {
                throw new InvalidOperationException(String.Format("Can not set start state {0} as is was not registered", startState));
            }

            State = startState;
        }

        private void AddState(TState fromState)
        {
            using var tracer = _tracer?.CreateScopeTracer();

            if (!m_States.ContainsKey(fromState))
            {
                m_States[fromState] = new StateConfiguration();
            }

            if (m_States.Count == 1)
            {
                SetStartState(fromState);
            }
        }

        public IStateMachineStateConfiguration<TState, TTrigger> Configure(TState fromState)
        {
            return new StateMachineStateConfiguration<TState, TTrigger>(this, fromState);
        }

        void IStateMachineConfiguration<TState,TTrigger>.AddEntryAction(TState fromState, Action entryAction, string description)
        {
            AddBasicEntryAction(fromState, new StateMachineAction<object>(entryAction, description));
        }

        void IStateMachineConfiguration<TState,TTrigger>.AddEntryAction<TTriggerArg>(TState fromState, Action<TTriggerArg> entryAction, string description)
        {
            AddBasicEntryAction(fromState, new StateMachineAction<TTriggerArg>(entryAction, description));
        }

        void IStateMachineConfiguration<TState, TTrigger>.AddEntryAction(TState fromState, TTrigger trigger, Action entryAction, string description)
        {
            AddSpecificEntryAction(fromState, trigger, new StateMachineAction<object>(entryAction, description));
        }

        void IStateMachineConfiguration<TState,TTrigger>.AddEntryAction<TTriggerArg>(TState fromState, TTrigger trigger, Action<TTriggerArg> entryAction, string description)
        {
            AddSpecificEntryAction(fromState, trigger, new StateMachineAction<TTriggerArg>(entryAction, description));
        }

        private void AddBasicEntryAction(TState fromState, IStateMachineAction action)
        {
            using var tracer = _tracer?.CreateScopeTracer();

            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            m_States[fromState].EntryActions.Add(action);
        }

        private void AddSpecificEntryAction(TState fromState, TTrigger trigger, IStateMachineAction action)
        {
            using var tracer = _tracer?.CreateScopeTracer();

            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            IList<IStateMachineAction> specificActions;
            if (!m_States[fromState].SpecificEntryActions.TryGetValue(trigger, out specificActions))
            {
                specificActions = new List<IStateMachineAction>();
                m_States[fromState].SpecificEntryActions[trigger] = specificActions;
            }

            specificActions.Add(action);
        }

        private IEnumerable<IStateMachineAction> GetEntryActions(TState fromState, TTrigger trigger)
        {
            using var tracer = _tracer?.CreateScopeTracer();

            var result = new List<IStateMachineAction>();

            IList<IStateMachineAction> specificActions;
            if (m_States[fromState].SpecificEntryActions.TryGetValue(trigger, out specificActions))
            {
                result.AddRange(specificActions);
            }

            result.AddRange(m_States[fromState].EntryActions);
            return result;
        }

        void IStateMachineConfiguration<TState, TTrigger>.AddLeaveAction(TState fromState, Action leaveAction, string description)
        {
            AddBasicLeaveAction(fromState, new StateMachineAction<object>(leaveAction, description));
        }

        void IStateMachineConfiguration<TState,TTrigger>.AddLeaveAction<TTriggerArg>(TState fromState, Action<TTriggerArg> leaveAction, string description)
        {
            AddBasicLeaveAction(fromState, new StateMachineAction<TTriggerArg>(leaveAction, description));
        }

        void IStateMachineConfiguration<TState, TTrigger>.AddLeaveAction(TState fromState, TTrigger trigger, Action leaveAction, string description)
        {
            AddSpecificLeaveAction(fromState, trigger, new StateMachineAction<object>(leaveAction, description));
        }

        void IStateMachineConfiguration<TState,TTrigger>.AddLeaveAction<TTriggerArg>(TState fromState, TTrigger trigger, Action<TTriggerArg> leaveAction, string description)
        {
            AddSpecificLeaveAction(fromState, trigger, new StateMachineAction<TTriggerArg>(leaveAction, description));
        }

        private void AddBasicLeaveAction(TState fromState, IStateMachineAction action)
        {
            using var tracer = _tracer?.CreateScopeTracer();

            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            m_States[fromState].LeaveActions.Add(action);
        }

        private void AddSpecificLeaveAction(TState fromState, TTrigger trigger, IStateMachineAction action)
        {
            using var tracer = _tracer?.CreateScopeTracer();

            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            IList<IStateMachineAction> specificActions;
            if (!m_States[fromState].SpecificLeaveActions.TryGetValue(trigger, out specificActions))
            {
                specificActions = new List<IStateMachineAction>();
                m_States[fromState].SpecificLeaveActions[trigger] = specificActions;
            }

            specificActions.Add(action);
        }

        private IEnumerable<IStateMachineAction> GetLeaveActions(TState fromState, TTrigger trigger)
        {
            using var tracer = _tracer?.CreateScopeTracer();

            var result = new List<IStateMachineAction>();

            result.AddRange(m_States[fromState].LeaveActions);

            IList<IStateMachineAction> specificActions;
            if (m_States[fromState].SpecificLeaveActions.TryGetValue(trigger, out specificActions))
            {
                result.AddRange(specificActions);
            }

            return result;
        }

        void IStateMachineConfiguration<TState, TTrigger>.AddTransition(TState fromState, TTrigger trigger, TState toState, string description)
        {
            AddTransition(fromState, trigger, new TriggerConfiguration<object>(toState, description));
        }

        void IStateMachineConfiguration<TState, TTrigger>.AddTransition(TState fromState, TTrigger trigger, TState toState, Func<bool> condition, string description)
        {
            AddTransition(fromState, trigger, new TriggerConfiguration<object>(toState, condition, description));
        }

        void IStateMachineConfiguration<TState, TTrigger>.AddTransition<TTriggerArg>(TState fromState, TTrigger trigger, TState toState, Func<TTriggerArg, bool> condition, string description)
        {
            AddTransition(fromState, trigger, new TriggerConfiguration<TTriggerArg>(toState, condition, description));
        }

        private void AddTransition(TState fromState, TTrigger trigger, ITriggerConfiguration triggerConfig)
        {
            using var tracer = _tracer?.CreateScopeTracer();

            if (triggerConfig == null)
            {
                throw new ArgumentNullException("triggerConfig");
            }

            var transitionKey = new Transition(fromState, trigger);

            ICollection<ITriggerConfiguration> triggerConfigs;
            if (!m_Transitions.TryGetValue(transitionKey, out triggerConfigs))
            {
                triggerConfigs = new List<ITriggerConfiguration>();
                m_Transitions.Add(transitionKey, triggerConfigs);
            }

            triggerConfigs.Add(triggerConfig);
        }


        private ITriggerConfiguration FindTransition<TTriggerArg>(TTrigger trigger, TTriggerArg triggerArg)
        {
            using var tracer = _tracer?.CreateScopeTracer();

            var transitionKey = new Transition(State, trigger);

            ICollection<ITriggerConfiguration> triggerConfigs;
            if (!m_Transitions.TryGetValue(transitionKey, out triggerConfigs))
            {
                return null;
            }

            if (triggerConfigs == null)
            {
                return null;
            }

            return triggerConfigs.FirstOrDefault(tc => tc != null && tc.CheckCondition(triggerArg));
        }

        public bool CanFire(TTrigger trigger)
        {
            return CanFire<object>(trigger,null);
        }

        public bool CanFire<TTriggerArg>(TTrigger trigger, TTriggerArg triggerArg)
        {
            using var tracer = _tracer?.CreateScopeTracer();
            return FindTransition(trigger, triggerArg) != null;
        }

        public bool Fire(TTrigger trigger)
        {
            return Fire<object>(trigger,null);
        }

        public bool Fire<TTriggerArg>(TTrigger trigger, TTriggerArg triggerArg)
        {
            using var tracer = _tracer?.CreateScopeTracer();

            var triggerConfiguration = FindTransition(trigger, triggerArg);

            if (triggerConfiguration == null)
            {
                // ReSharper disable once CompareNonConstrainedGenericWithNull
                var messageFormat = triggerArg != null 
                    ? "No transition configured for trigger {1} on state {0} with object {2}" 
                    : "No transition configured for trigger {1} on state {0}";
                var message = string.Format(messageFormat, State, trigger, triggerArg);

                tracer?.Info(message);

                if (UnexpectedTriggerHandler != null)
                {
                    UnexpectedTriggerHandler(State, trigger, message);
                }
                return false;
            }

            var fromState = State;
            var toState = triggerConfiguration.ToState;

            tracer?.Info($"Valid transition found for trigger {fromState} on state {trigger} with object {triggerArg} leading to state {toState}");

            try
            {
                tracer?.Info($"Leaving state {State} (object: {triggerArg})");
                foreach (var leaveAction in GetLeaveActions(State, trigger))
                {
                    leaveAction.Execute(triggerArg);
                }

                tracer?.Info($"Changing state from {State} to {toState}");
                State = toState;

                tracer?.Info($"Entering state {State} (object: {triggerArg})");
                foreach (var entryAction in GetEntryActions(State, trigger))
                {
                    entryAction.Execute(triggerArg);
                }

                if (StateTransitionCompletedHandler != null)
                {
                    StateTransitionCompletedHandler(fromState, toState, trigger, triggerArg, typeof(TTriggerArg));
                }
            }
            catch(Exception ex)
            {
                if (StateTransitionExceptionHandler != null)
                {
                    StateTransitionExceptionHandler(
                        fromState, 
                        toState, 
                        ex,
                        newState => { State = newState; },
                        newState =>
                        {
                            if (!State.Equals(newState))
                            {
                                try
                                {
                                    foreach (var leaveAction in GetLeaveActions(State, trigger))
                                    {
                                        leaveAction.Execute(triggerArg);
                                    }
                                    State = newState;
                                    foreach (var entryAction in GetEntryActions(State, trigger))
                                    {
                                        entryAction.Execute(triggerArg);
                                    }
                                }
                                finally
                                {
                                    State = newState;
                                }
                            }
                        });
                }
                else
                {
                    throw;
                }
            }

            return true;
        }

        /// <summary>
        /// Provides an info object which exposes the states, transitions, and actions of this machine.
        /// </summary>
        public IStateMachineInfo GetInfo()
        {
            var stateInfos = m_States.ToDictionary(kvp => kvp.Key, kvp => GetStateInfo(kvp.Key, kvp.Value));

            foreach (var kvp in stateInfos)
            {
                AddRelationships(kvp.Key, kvp.Value);
            }

            return new StateMachineInfo(stateInfos.Values, typeof(TState), typeof(TTrigger));
        }

        private StateInfo GetStateInfo(TState stateKey, StateConfiguration stateConfig)
        {
            var entryActions = stateConfig.EntryActions.Select(act => act.Description);
            var specificEntryActions = stateConfig.SpecificEntryActions.SelectMany(kvp => kvp.Value.Select(act => string.Format("{0} ({1})", act.Description, kvp.Key)));

            var exitActions = stateConfig.LeaveActions.Select(act => act.Description);
            var specificExitActions = stateConfig.SpecificLeaveActions.SelectMany(kvp => kvp.Value.Select(act => string.Format("{0} ({1})", act.Description, kvp.Key)));

            var ignoredTriggers = new List<ITriggerInfo>();

            return new StateInfo(stateKey, specificEntryActions.Union(entryActions), exitActions.Union(specificExitActions), ignoredTriggers);
        }

        private void AddRelationships(TState stateKey, StateInfo stateInfo)
        {
            if (stateInfo == null)
            {
                throw new ArgumentNullException("stateInfo");
            }

            if (!stateKey.Equals(stateInfo.UnderlyingState))
            {
                throw new ArgumentException("stateKey does not match stateInfo");
            }

            IStateInfo superState = null;
            IEnumerable<IStateInfo> subStates = new IStateInfo[] { };

            var stateTransitions = m_Transitions
                .Where(kvp => kvp.Key.FromState.Equals(stateKey))
                .SelectMany(kvp => kvp.Value.Select(tr => new KeyValuePair<Transition, ITriggerConfiguration>(kvp.Key, tr)));

            //var unconditionedTransitions = stateTransitions.Where(kvp => kvp.Value.CheckCondition(null));
            //var conditionedTransitions = stateTransitions.Where(kvp => kvp.Value.CheckCondition(null) == false);

            IEnumerable<IFixedTransitionInfo> fixedTransitions = stateTransitions
                .Select(kvp => 
                    new FixedTransitionInfo(
                        triggerInfo: new TriggerInfo(kvp.Key.Trigger),
                        guardDescription: kvp.Value.Description,
                        destinationStateInfo: GetStateInfo(kvp.Value.ToState, m_States[kvp.Value.ToState])));

            // we currently do not support dynamic transitions (transitions where the target state depends on a function provided in the trigger configuration)
            //IEnumerable<IDynamicTransitionInfo> dynamicTransitions = dynamicTransitions
            //    .Select(kvp =>
            //        new DynamicTransitionInfo(
            //            triggerInfo: new TriggerInfo(kvp.Key.Trigger),
            //            guardDescription: kvp.Value.Description,
            //            destinationStateInfo: kvp.Value.ToStateCalculator().ToString()));

            // we currently do not support SuperStates with SubStates -> superState is always null
            // ReSharper disable once ExpressionIsAlwaysNull
            stateInfo.AddRelationships(superState, subStates, fixedTransitions, new IDynamicTransitionInfo[]{});
        }


        private class Transition : IEquatable<Transition>
        {
            public TState FromState { get; private set; }
            public TTrigger Trigger { get; private set; }

            public Transition(TState fromState, TTrigger trigger)
            {
                FromState = fromState;
                Trigger = trigger;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (EqualityComparer<TState>.Default.GetHashCode(FromState)*397) ^ EqualityComparer<TTrigger>.Default.GetHashCode(Trigger);
                }
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                {
                    return false;
                }
                if (ReferenceEquals(this, obj))
                {
                    return true;
                }
                if (obj.GetType() != GetType())
                {
                    return false;
                }
                return Equals((Transition) obj);
            }

            public bool Equals(Transition other)
            {
                if (ReferenceEquals(null, other))
                {
                    return false;
                }
                if (ReferenceEquals(this, other))
                {
                    return true;
                }
                return EqualityComparer<TState>.Default.Equals(FromState, other.FromState) && EqualityComparer<TTrigger>.Default.Equals(Trigger, other.Trigger);
            }
        }

        private interface ITriggerConfiguration
        {
            string Description { get; }
            TState ToState { get; }
            bool CheckCondition(object arg);
        }

        private class TriggerConfiguration<TTriggerArg> : ITriggerConfiguration
        {
            public string Description { get; private set; }
            public TState ToState { get; private set; }
            private  Func<object, bool> Condition { get; set; }

            public bool CheckCondition(object arg)
            {
                return Condition(arg);
            }

            public TriggerConfiguration(TState toState, string description)
            {
                description = string.IsNullOrEmpty(description) ? string.Empty : description;

                ToState = toState;
                Condition = _ => true;
                Description = description;
            }

            public TriggerConfiguration(TState toState, Func<bool> condition, string description)
            {
                if (condition == null)
                {
                    throw new ArgumentNullException("condition");
                }

                description = string.IsNullOrEmpty(description) ? condition.GetMethodInfo().Name : description;

                ToState = toState;
                Condition = o => condition();
                Description = description;
            }

            public TriggerConfiguration(TState toState, Func<TTriggerArg, bool> condition, string description)
            {
                if (condition == null)
                {
                    throw new ArgumentNullException("condition");
                }

                description = string.IsNullOrEmpty(description) ? condition.GetMethodInfo().Name : description;

                ToState = toState;
                Condition = o => condition((TTriggerArg)o);
                Description = description;
            }
        }

        private class StateConfiguration
        {
            public IList<IStateMachineAction> EntryActions { get; private set; }
            public IDictionary<TTrigger, IList<IStateMachineAction>> SpecificEntryActions { get; private set; }
            public IList<IStateMachineAction> LeaveActions { get; private set; }
            public IDictionary<TTrigger, IList<IStateMachineAction>> SpecificLeaveActions { get; private set; }

            public StateConfiguration()
            {
                EntryActions = new List<IStateMachineAction>();
                SpecificEntryActions = new Dictionary<TTrigger, IList<IStateMachineAction>>();
                LeaveActions = new List<IStateMachineAction>();
                SpecificLeaveActions = new Dictionary<TTrigger, IList<IStateMachineAction>>();
            }
        }

        private interface IStateMachineAction
        {
            string Description { get; }
            void Execute(object arg);
        }

        private class StateMachineAction<TTriggerArg> : IStateMachineAction
        {
            public string Description { get; private set; }
            private Action<object> Action { get; set; }

            public void Execute(object arg)
            {
                Action.Invoke(arg);
            }

            public StateMachineAction(Action action, string description)
            {
                if (action == null)
                {
                    throw new ArgumentNullException("action");
                }

                description = string.IsNullOrEmpty(description) ? action.GetMethodInfo().Name : description;

                Action = o => action();
                Description = description;
            }

            public StateMachineAction(Action<TTriggerArg> action, string description)
            {
                if (action == null)
                {
                    throw new ArgumentNullException("action");
                }
                
                description = string.IsNullOrEmpty(description) ? action.GetMethodInfo().Name : description;

                Action = o => action((TTriggerArg)o);
                Description = description;
            }
        }
    
    }
}