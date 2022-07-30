//----------------------------------------------------------------------------------
// <copyright file="IStateMachine.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2016-2018. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

using System;

namespace Solid.Infrastructure.StateMachine
{
    /// <summary>
    /// IStateMachine{TState, TTrigger}
    /// </summary>
    public interface IStateMachine<TState, TTrigger>
        where TState : struct, IConvertible, IComparable, IFormattable
        where TTrigger : struct, IConvertible, IComparable, IFormattable
    {
        /// <summary>
        /// Starts the configuration of a specific state
        /// i.e. defining transitions to other states depending on triggers and specifying entry ane exit handlers
        /// </summary>
        /// <param name="state">The state to configure.</param>
        /// <returns>A configuration object through which the state can be configured.</returns>
        IStateMachineStateConfiguration<TState, TTrigger> Configure(TState state);

        /// <summary>
        /// The current state
        /// </summary>
        TState State { get; }

        /// <summary>
        /// SetStartState
        /// </summary>
        void SetStartState(TState startState);

        /// <summary>
        /// IsInState
        /// </summary>
        /// <param name="state">The state to test for.</param>
        /// <returns>True if the current state is equal to, or a substate of the supplied state.</returns>
        bool IsInState(TState state);

        /// <summary>
        /// Triggers the state machine to transit from current state into another state.
        /// * The target state is determined by the configuration of the current state
        ///   <see cref="IStateMachineStateConfiguration{TState,TTrigger}.Permit"/>
        /// * Actions associated with leaving the current state and entering the new state will be invoked.
        ///   <see cref="IStateMachineStateConfiguration{TState,TTrigge}.OnEntry(Action,string)"/>
        /// Handling of the trigger argument:
        /// * Conditions associated to the transition for that trigger, will recieve the trigger parameter as input argument
        ///   <see cref="IStateMachineStateConfiguration{TState,TTrigger}.PermitIf"/>
        /// * Actions associated to the transition specific for that trigger, will receive the trigger parameter as input argument
        ///   <see cref="IStateMachineStateConfiguration{TState,TTrigger}.OnEntryFrom"/> and  <see cref="IStateMachineStateConfiguration{TState,TTrigger}.OnExitFrom"/>
        /// * Generic actions associated for the transition which expect an argument, will recive the trigger parameter as input argument if the types are compatible 
        ///   <see cref="IStateMachineStateConfiguration{TState,TTrigge}.OnEntry(Action{object},string)"/>
        /// </summary>
        /// <param name="trigger">The trigger to fire.</param>
        /// <param name="triggerArg">The parameter associated to the trigger.</param>
        /// <returns>
        /// True : if the trigger was configured for the current state and hence the transition was executed
        /// False: otherwise + the UnexpectedTriggerHandler will be invoked
        /// </returns>
        bool Fire<TTriggerArg>(TTrigger trigger, TTriggerArg triggerArg);

        /// <summary>
        /// Same as <see cref="Fire{TTriggerArg}"/> but without a trigger parameter
        /// </summary>
        /// <param name="trigger">The trigger to fire.</param>
        /// <returns>
        /// True : if for the current state a transition was configured for that trigger
        /// False: otherwise + the UnexpectedTriggerHandler will be invoked
        /// </returns>
        bool Fire(TTrigger trigger);

        /// <summary>
        /// Same as <see cref="Fire{TTriggerArg}"/> but without executiong the transition
        /// </summary>
        /// <returns>
        /// True : if for the current state a transition was configured for that trigger
        /// False: otherwise
        /// </returns>
        bool CanFire<TTriggerArg>(TTrigger trigger, TTriggerArg triggerArg);
        
        /// <summary>
        /// Same as <see cref="Fire"/> but without executiong the transition
        /// </summary>
        /// <returns>
        /// True : if for the current state a transition was configured for that trigger
        /// False: otherwise
        /// </returns>
        bool CanFire(TTrigger trigger);

        /// <summary>
        /// Error handler for unexpected triggers
        /// handler signature: 
        /// private void UnexpectedTriggerHandler(States state, Triggers trigger, string message);
        /// </summary>
        Action<TState, TTrigger, string> UnexpectedTriggerHandler { set; }

        /// <summary>
        /// Error handler for exceptions that occure in code executed during state transition
        /// handler signature: 
        /// private void StateTransitionExceptionHandler(States fromState, States toState, Exception exception, Action{TState} setStateAction, Action{TState} gotoStateAction);
        /// </summary>
        Action<TState, TState, Exception, Action<TState>, Action<TState>> StateTransitionExceptionHandler { set; }

        /// <summary>
        /// Notification handler for successfully executed state transition
        /// handler signature: 
        /// private void StateTransitionCompleted(States fromState, States toState, Triggers trigger, object triggerArg, Type triggerArgType);
        /// </summary>
        Action<TState, TState, TTrigger, object, Type> StateTransitionCompletedHandler { set; }

        /// <summary>
        /// Provides an info object which exposes the states, transitions, and actions of this machine.
        /// </summary>
        IStateMachineInfo GetInfo();
    }
}