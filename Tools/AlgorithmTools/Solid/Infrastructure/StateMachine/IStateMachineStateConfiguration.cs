//----------------------------------------------------------------------------------
// <copyright file="IStateMachineStateConfiguration.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2017-2018. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

using System;

namespace Solid.Infrastructure.StateMachine
{
    /// <summary>
    /// IStateMachineStateConfiguration{TState, TTrigger}
    /// </summary>
    public interface IStateMachineStateConfiguration<in TState, in TTrigger>
        where TState : struct, IConvertible, IComparable, IFormattable
        where TTrigger : struct, IConvertible, IComparable, IFormattable
    {
        IStateMachineStateConfiguration<TState, TTrigger> AsStartState();

        IStateMachineStateConfiguration<TState, TTrigger> OnEntry(Action entryAction, string description = null);
        IStateMachineStateConfiguration<TState, TTrigger> OnEntry(Action<object> entryAction, string description = null);
        IStateMachineStateConfiguration<TState, TTrigger> OnEntryFrom(TTrigger trigger, Action entryAction, string description = null);
        IStateMachineStateConfiguration<TState, TTrigger> OnEntryFrom<TTriggerArg>(TTrigger trigger, Action<TTriggerArg> entryAction, string description = null);

        IStateMachineStateConfiguration<TState, TTrigger> OnExit(Action leaveAction, string description = null);
        IStateMachineStateConfiguration<TState, TTrigger> OnExit(Action<object> leaveAction, string description = null);
        IStateMachineStateConfiguration<TState, TTrigger> OnExitFrom(TTrigger trigger, Action leaveAction, string description = null);
        IStateMachineStateConfiguration<TState, TTrigger> OnExitFrom<TTriggerArg>(TTrigger trigger, Action<TTriggerArg> leaveAction, string description = null);

        IStateMachineStateConfiguration<TState, TTrigger> Permit(TTrigger trigger, TState toState, string description = null);
        IStateMachineStateConfiguration<TState, TTrigger> PermitIf(TTrigger trigger, TState toState, Func<bool> condition, string description = null);
        IStateMachineStateConfiguration<TState, TTrigger> PermitIf<TTriggerArg>(TTrigger trigger, TState toState, Func<TTriggerArg, bool> condition, string description = null);

        IStateMachineStateConfiguration<TState, TTrigger> PermitReentry(TTrigger trigger, string description = null);
        IStateMachineStateConfiguration<TState, TTrigger> PermitReentryIf(TTrigger trigger, Func<bool> condition, string description = null);
        IStateMachineStateConfiguration<TState, TTrigger> PermitReentryIf<TTriggerArg>(TTrigger trigger, Func<TTriggerArg, bool> condition, string description = null);
    }
}