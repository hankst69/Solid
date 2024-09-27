//----------------------------------------------------------------------------------
// File: "StateMachineStateConfiguration.cs"
// Author: Steffen Hanke
// Date: 2016-2018
//----------------------------------------------------------------------------------
using System;

namespace Solid.Infrastructure.StateMachine.Impl
{
    public class StateMachineStateConfiguration<TState, TTrigger> : 
        IStateMachineStateConfiguration<TState, TTrigger>
        where TState : struct, IConvertible, IComparable, IFormattable
        where TTrigger : struct, IConvertible, IComparable, IFormattable
    {
        internal StateMachineStateConfiguration(IStateMachineConfiguration<TState, TTrigger> stateMachineConfiguration, TState fromState)
        {
            if (stateMachineConfiguration == null)
                throw new ArgumentNullException("stateMachineConfiguration");

            if (!typeof(TState).IsEnum)
            {
                throw new ArgumentException("TState must be an enum.");
            }
            if (!typeof(TTrigger).IsEnum)
            {
                throw new ArgumentException("TTrigger must be an enum.");
            }

            StateMachineConfiguration = stateMachineConfiguration;
            FromState = fromState;
        }

        public TState FromState { get; private set; }
        internal IStateMachineConfiguration<TState, TTrigger> StateMachineConfiguration { get; private set; }

        public IStateMachineStateConfiguration<TState, TTrigger> AsStartState()
        {
            StateMachineConfiguration.SetStartState(FromState);
            return this;
        }



        public IStateMachineStateConfiguration<TState, TTrigger> OnEntry(Action entryAction, string description)
        {
            StateMachineConfiguration.AddEntryAction(FromState, entryAction, description);
            return this;
        }

        public IStateMachineStateConfiguration<TState, TTrigger> OnEntry(Action<object> entryAction, string description)
        {
            StateMachineConfiguration.AddEntryAction(FromState, entryAction, description);
            return this;
        }

        public IStateMachineStateConfiguration<TState, TTrigger> OnEntryFrom(TTrigger trigger, Action entryAction, string description)
        {
            StateMachineConfiguration.AddEntryAction(FromState, trigger, entryAction, description);
            return this;
        }

        public IStateMachineStateConfiguration<TState, TTrigger> OnEntryFrom<TTriggerArg>(TTrigger trigger, Action<TTriggerArg> entryAction, string description)
        {
            StateMachineConfiguration.AddEntryAction(FromState, trigger, entryAction, description);
            return this;
        }



        public IStateMachineStateConfiguration<TState, TTrigger> OnExit(Action leaveAction, string description)
        {
            StateMachineConfiguration.AddLeaveAction(FromState, leaveAction, description);
            return this;
        }

        public IStateMachineStateConfiguration<TState, TTrigger> OnExit(Action<object> leaveAction, string description)
        {
            StateMachineConfiguration.AddLeaveAction(FromState, leaveAction, description);
            return this;
        }

        public IStateMachineStateConfiguration<TState, TTrigger> OnExitFrom(TTrigger trigger, Action leaveAction, string description)
        {
            StateMachineConfiguration.AddLeaveAction(FromState, trigger, leaveAction, description);
            return this;
        }

        public IStateMachineStateConfiguration<TState, TTrigger> OnExitFrom<TTriggerArg>(TTrigger trigger, Action<TTriggerArg> leaveAction, string description)
        {
            StateMachineConfiguration.AddLeaveAction(FromState, trigger, leaveAction, description);
            return this;
        }



        public IStateMachineStateConfiguration<TState, TTrigger> Permit(TTrigger trigger, TState toState, string description)
        {
            StateMachineConfiguration.AddTransition(FromState, trigger, toState, description);
            return this;
        }

        public IStateMachineStateConfiguration<TState, TTrigger> PermitIf(TTrigger trigger, TState toState, Func<bool> condition, string description)
        {
            StateMachineConfiguration.AddTransition(FromState, trigger, toState, condition, description);
            return this;
        }

        public IStateMachineStateConfiguration<TState, TTrigger> PermitIf<TTriggerArg>(TTrigger trigger, TState toState, Func<TTriggerArg, bool> condition, string description)
        {
            StateMachineConfiguration.AddTransition(FromState, trigger, toState, condition, description);
            return this;
        }



        public IStateMachineStateConfiguration<TState, TTrigger> PermitReentry(TTrigger trigger, string description)
        {
            return Permit(trigger, FromState, description);
        }

        public IStateMachineStateConfiguration<TState, TTrigger> PermitReentryIf(TTrigger trigger, Func<bool> condition, string description)
        {
            return PermitIf(trigger, FromState, condition, description);
        }

        public IStateMachineStateConfiguration<TState, TTrigger> PermitReentryIf<TTriggerArg>(TTrigger trigger, Func<TTriggerArg, bool> condition, string description)
        {
            return PermitIf(trigger, FromState, condition, description);
        }
    }
}