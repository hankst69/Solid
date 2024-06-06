//----------------------------------------------------------------------------------
// File: "StateMachineTests.cs"
// Author: Steffen Hanke
// Date: 2018-2019
//----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using Solid.Infrastructure.Diagnostics;
using Solid.Infrastructure.StateMachine;
using Solid.Infrastructure.StateMachine.Impl;
using NUnit.Framework;

namespace Solid.Infrastructure_uTest.StateMachine
{
    public class StateMachineTests
    {
        enum State
        {
            A, B, C
        }

        enum Trigger
        {
            X, Y, Z
        }

        [Test]
        public void Ctor_ShouldThrow_WhenStatesTypeIsNotEnum()
        {
            // Arrange
            // Act
            Action action = () => new StateMachine<int, Trigger>(); 

            // Assert
            action.Should().Throw<ArgumentException>().WithMessage("TState must be an enum.");
        }

        [Test]
        public void Ctor_ShouldThrow_WhenTriggerssTypeIsNotEnum()
        {
            // Arrange
            // Act
            Action action = () => new StateMachine<State, long>();

            // Assert
            action.Should().Throw<ArgumentException>().WithMessage("TTrigger must be an enum.");
        }

        [Test]
        public void Ctor_ShouldSetInitialState()
        {
            // Arrange
            // Act
            var initial = State.B;
            var sm = new StateMachine<State, Trigger>(initial);

            // Assert
            sm.State.Should().Be(initial);
        }

        [Test]
        public void Ctor_ShouldNotThrow()
        {
            // Arrange
            // Act
            Action action = () => new StateMachine<State, Trigger>();

            // Assert
            action.Should().NotThrow<ArgumentNullException>();
        }

        [Test]
        public void Ctor_ShouldNotThrow_WhenTracerIsNull()
        {
            // Arrange
            // Act
            Action action = () => new StateMachine<State, Trigger>((ITracer)null);

            // Assert
            action.Should().NotThrow<ArgumentNullException>();
        }

        [Test]
        public void Ctor_ShouldNotThrow_WhenInitialStateGivenAndTracerIsNull()
        {
            // Arrange
            // Act
            var initial = State.B;
            IStateMachine<State, Trigger> sm = null;
            Action action = () => sm = new StateMachine<State, Trigger>(initial, (ITracer) null);

            // Assert
            action.Should().NotThrow<ArgumentNullException>();
            sm.State.Should().Be(initial);
        }

        [Test, Ignore("simplified ITracer usage - test does not apply anymore")]
        public void Ctor_ShouldAdjustTracerDomainToItsFullName()
        {
            // Arrange
            var tracerMock = new Mock<ITracer>();
            tracerMock.Setup(x => x.CreateBaseDomainTracer(It.IsAny<Type>())).Returns(tracerMock.Object);

            // Act
            var target = new StateMachine<State, Trigger>(tracerMock.Object);

            // Assert
            tracerMock.Verify(x => x.CreateBaseDomainTracer(It.Is<Type>(ty => ty.Equals(target.GetType()))), Times.Once);
        }

        [Test]
        public void Fire_ShouldNotThrow_WhenTargetStateIsNotConfigured()
        {
            // with StateMichine implementataion up to November 2017, the possible states were not preconfigured
            // therefore a transition to an unconfigured state caused a KeyNotFoundException
            // meanwhile this is fixed and this test will ensure the fix is working

            // Arrange
            var sm = new StateMachine<State, Trigger>(State.A);
            sm.Configure(State.A).Permit(Trigger.X, State.B);

            // Act
            Action action = () => sm.Fire(Trigger.X);

            // Assert
            action.Should().NotThrow<KeyNotFoundException>();
        }

        [Test]
        public void Fire_ShouldReturnFalse_WhenTransitionIsNotConfigured()
        {
            // Arrange
            var sm = new StateMachine<State, Trigger>(State.A);
            //sm.Configure(State.A).Permit(Trigger.X, State.B);

            // Act
            var result = sm.Fire(Trigger.X);

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void StateMachine_SimpleTest()
        {
            // Arrange
            var states = Enum.GetValues(typeof(State)).Cast<State>().ToArray();
            var triggers = Enum.GetValues(typeof (Trigger)).Cast<Trigger>();
            var a = states.First();
            var b = states.Skip(1).First();
            var x = triggers.First();
            var sm = new StateMachine<State, Trigger>(a);
            sm.Configure(a).Permit(x, b);
            sm.Configure(b);

            // Act
            sm.Fire(x);

            // Assert
            sm.State.Should().Be(b);
        }


        /*
        private void RunSimpleTest<TState, TTrigger>(IEnumerable<TState> states, IEnumerable<TTrigger> transitions)
        {
            var a = states.First();
            var b = states.Skip(1).First();
            var x = transitions.First();

            var sm = new StateMachine<TState, TTrigger>(a);

            sm.Configure(a)
                .Permit(x, b);

            sm.Fire(x);

            b.Should().ShouldBeEquivalentTo(sm.State);
        }
        [Test]
        public void StateMachine_ShouldBeAbleToUseReferenceTypeMarkers()
        {
            const string
                StateA = "A", StateB = "B", StateC = "C",
                TriggerX = "X", TriggerY = "Y";

            RunSimpleTest(
                new[] { StateA, StateB, StateC },
                new[] { TriggerX, TriggerY });
        }
        [Test]
        public void StateMachine_ShouldBeAbleToUseValueTypeMarkers()
        {
            RunSimpleTest(
                Enum.GetValues(typeof(State)).Cast<State>(),
                Enum.GetValues(typeof(Trigger)).Cast<Trigger>());
        }
  
        [Test]
        public void StateCanBeStoredExternally()
        {
            var state = State.B;
            var sm = new StateMachine<State, Trigger>(() => state, s => state = s);
            sm.Configure(State.B).Permit(Trigger.X, State.C);
            Assert.Equal(State.B, sm.State);
            Assert.Equal(State.B, state);
            sm.Fire(Trigger.X);
            Assert.Equal(State.C, sm.State);
            Assert.Equal(State.C, state);
        }

        [Test]
        public void SubstateIsIncludedInCurrentState()
        {
            var sm = new StateMachine<State, Trigger>(State.B);
            sm.Configure(State.B).SubstateOf(State.C);

            Assert.Equal(State.B, sm.State);
            Assert.True(sm.IsInState(State.C));
        }

        [Test]
        public void WhenInSubstate_TriggerIgnoredInSuperstate_RemainsInSubstate()
        {
            var sm = new StateMachine<State, Trigger>(State.B);

            sm.Configure(State.B)
                .SubstateOf(State.C);

            sm.Configure(State.C)
                .Ignore(Trigger.X);

            sm.Fire(Trigger.X);

            Assert.Equal(State.B, sm.State);
        }

        [Test]
        public void PermittedTriggersIncludeSuperstatePermittedTriggers()
        {
            var sm = new StateMachine<State, Trigger>(State.B);

            sm.Configure(State.A)
                .Permit(Trigger.Z, State.B);

            sm.Configure(State.B)
                .SubstateOf(State.C)
                .Permit(Trigger.X, State.A);

            sm.Configure(State.C)
                .Permit(Trigger.Y, State.A);

            var permitted = sm.PermittedTriggers;

            Assert.True(permitted.Contains(Trigger.X));
            Assert.True(permitted.Contains(Trigger.Y));
            Assert.False(permitted.Contains(Trigger.Z));
        }

        [Test]
        public void PermittedTriggersAreDistinctValues()
        {
            var sm = new StateMachine<State, Trigger>(State.B);

            sm.Configure(State.B)
                .SubstateOf(State.C)
                .Permit(Trigger.X, State.A);

            sm.Configure(State.C)
                .Permit(Trigger.X, State.B);

            var permitted = sm.PermittedTriggers;
            Assert.Equal(1, permitted.Count());
            Assert.Equal(Trigger.X, permitted.First());
        }

        [Test]
        public void AcceptedTriggersRespectGuards()
        {
            var sm = new StateMachine<State, Trigger>(State.B);

            sm.Configure(State.B)
                .PermitIf(Trigger.X, State.A, () => false);

            Assert.Equal(0, sm.PermittedTriggers.Count());
        }

        [Test]
        public void AcceptedTriggersRespectMultipleGuards()
        {
            var sm = new StateMachine<State, Trigger>(State.B);

            sm.Configure(State.B)
                .PermitIf(Trigger.X, State.A,
                    new Tuple<Func<bool>, string>(() => true, "1"),
                    new Tuple<Func<bool>, string>(() => false, "2"));

            Assert.Equal(0, sm.PermittedTriggers.Count());
        }

        [Test]
        public void WhenDiscriminatedByGuard_ChoosesPermitedTransition()
        {
            var sm = new StateMachine<State, Trigger>(State.B);

            sm.Configure(State.B)
                .PermitIf(Trigger.X, State.A, () => false)
                .PermitIf(Trigger.X, State.C, () => true);

            sm.Fire(Trigger.X);

            Assert.Equal(State.C, sm.State);
        }

        [Test]
        public void WhenDiscriminatedByMultiConditionGuard_ChoosesPermitedTransition()
        {
            var sm = new StateMachine<State, Trigger>(State.B);

            sm.Configure(State.B)
                .PermitIf(Trigger.X, State.A,
                    new Tuple<Func<bool>, string>(() => true, "1"),
                    new Tuple<Func<bool>, string>(() => false, "2"))
                .PermitIf(Trigger.X, State.C,
                    new Tuple<Func<bool>, string>(() => true, "1"),
                    new Tuple<Func<bool>, string>(() => true, "2"));

            sm.Fire(Trigger.X);

            Assert.Equal(State.C, sm.State);
        }

        [Test]
        public void WhenTriggerIsIgnored_ActionsNotExecuted()
        {
            var sm = new StateMachine<State, Trigger>(State.B);

            bool fired = false;

            sm.Configure(State.B)
                .OnEntry(t => fired = true)
                .Ignore(Trigger.X);

            sm.Fire(Trigger.X);

            Assert.False(fired);
        }

        [Test]
        public void IfSelfTransitionPermited_ActionsFire()
        {
            var sm = new StateMachine<State, Trigger>(State.B);

            bool fired = false;

            sm.Configure(State.B)
                .OnEntry(t => fired = true)
                .PermitReentry(Trigger.X);

            sm.Fire(Trigger.X);

            Assert.True(fired);
        }

        [Test]
        public void ImplicitReentryIsDisallowed()
        {
            var sm = new StateMachine<State, Trigger>(State.B);

            Assert.Throws<ArgumentException>(() => sm.Configure(State.B)
               .Permit(Trigger.X, State.B));
        }

        [Test]
        public void TriggerParametersAreImmutableOnceSet()
        {
            var sm = new StateMachine<State, Trigger>(State.B);
            sm.SetTriggerParameters<string, int>(Trigger.X);
            Assert.Throws<InvalidOperationException>(() => sm.SetTriggerParameters<string>(Trigger.X));
        }

        [Test]
        public void ExceptionThrownForInvalidTransition()
        {
            var sm = new StateMachine<State, Trigger>(State.A);
            var exception = Assert.Throws<InvalidOperationException>(() => sm.Fire(Trigger.X));
            Assert.Equal(exception.Message, "No valid leaving transitions are permitted from state 'A' for trigger 'X'. Consider ignoring the trigger.");
        }

        [Test]
        public void ExceptionThrownForInvalidTransitionMentionsGuardDescriptionIfPresent()
        {
            // If guard description is empty then method name of guard is used
            // so I have skipped empty description test.
            const string guardDescription = "test";

            var sm = new StateMachine<State, Trigger>(State.A);
            sm.Configure(State.A).PermitIf(Trigger.X, State.B, () => false, guardDescription);
            var exception = Assert.Throws<InvalidOperationException>(() => sm.Fire(Trigger.X));
            Assert.Equal(exception.Message, "Trigger 'X' is valid for transition from state 'A' but a guard conditions are not met. Guard descriptions: 'test'.");
        }

        [Test]
        public void ExceptionThrownForInvalidTransitionMentionsMultiGuardGuardDescriptionIfPresent()
        {
            var sm = new StateMachine<State, Trigger>(State.A);
            sm.Configure(State.A).PermitIf(Trigger.X, State.B,
                new Tuple<Func<bool>, string>(() => false, "test1"),
                new Tuple<Func<bool>, string>(() => false, "test2"));

            var exception = Assert.Throws<InvalidOperationException>(() => sm.Fire(Trigger.X));
            Assert.Equal(exception.Message, "Trigger 'X' is valid for transition from state 'A' but a guard conditions are not met. Guard descriptions: 'test1, test2'.");
        }

        [Test]
        public void ParametersSuppliedToFireArePassedToEntryAction()
        {
            var sm = new StateMachine<State, Trigger>(State.B);

            var x = sm.SetTriggerParameters<string, int>(Trigger.X);

            sm.Configure(State.B)
                .Permit(Trigger.X, State.C);

            string entryArgS = null;
            int entryArgI = 0;

            sm.Configure(State.C)
                .OnEntryFrom(x, (s, i) =>
                {
                    entryArgS = s;
                    entryArgI = i;
                });

            var suppliedArgS = "something";
            var suppliedArgI = 42;

            sm.Fire(x, suppliedArgS, suppliedArgI);

            Assert.Equal(suppliedArgS, entryArgS);
            Assert.Equal(suppliedArgI, entryArgI);
        }

        [Test]
        public void WhenAnUnhandledTriggerIsFired_TheProvidedHandlerIsCalledWithStateAndTrigger()
        {
            var sm = new StateMachine<State, Trigger>(State.B);

            State? state = null;
            Trigger? trigger = null;
            sm.OnUnhandledTrigger((s, t, u) =>
                                      {
                                          state = s;
                                          trigger = t;
                                      });

            sm.Fire(Trigger.Z);

            Assert.Equal(State.B, state);
            Assert.Equal(Trigger.Z, trigger);
        }

        [Test]
        public void WhenATransitionOccurs_TheOnTransitionEventFires()
        {
            var sm = new StateMachine<State, Trigger>(State.B);

            sm.Configure(State.B)
                .Permit(Trigger.X, State.A);

            StateMachine<State, Trigger>.Transition transition = null;
            sm.OnTransitioned(t => transition = t);

            sm.Fire(Trigger.X);

            Assert.NotNull(transition);
            Assert.Equal(Trigger.X, transition.Trigger);
            Assert.Equal(State.B, transition.Source);
            Assert.Equal(State.A, transition.Destination);
        }

        [Test]
        public void TheOnTransitionEventFiresBeforeTheOnEntryEvent()
        {
            var sm = new StateMachine<State, Trigger>(State.B);
            var expectedOrdering = new List<string> { "OnExit", "OnTransitioned", "OnEntry" };
            var actualOrdering = new List<string>();

            sm.Configure(State.B)
                .Permit(Trigger.X, State.A)
                .OnExit(() => actualOrdering.Add("OnExit"));

            sm.Configure(State.A)
                .OnEntry(() => actualOrdering.Add("OnEntry"));

            sm.OnTransitioned(t => actualOrdering.Add("OnTransitioned"));

            sm.Fire(Trigger.X);

            Assert.Equal(expectedOrdering.Count, actualOrdering.Count);
            for (int i = 0; i < expectedOrdering.Count; i++)
            {
                Assert.Equal(expectedOrdering[i], actualOrdering[i]);
            }
        }

        [Test]
        public void DirectCyclicConfigurationDetected()
        {
            var sm = new StateMachine<State, Trigger>(State.A);

            Assert.Throws(typeof(ArgumentException),  () => { sm.Configure(State.A).SubstateOf(State.A); });
        }

        [Test]
        public void NestedCyclicConfigurationDetected()
        {
            var sm = new StateMachine<State, Trigger>(State.A);
            sm.Configure(State.B).SubstateOf(State.A);

            Assert.Throws(typeof(ArgumentException), () => { sm.Configure(State.A).SubstateOf(State.B); });
        }

        [Test]
        public void NestedTwoLevelsCyclicConfigurationDetected()
        {
            var sm = new StateMachine<State, Trigger>(State.A);
            sm.Configure(State.B).SubstateOf(State.A);
            sm.Configure(State.C).SubstateOf(State.B);

            Assert.Throws(typeof(ArgumentException), () => { sm.Configure(State.A).SubstateOf(State.C); });
        }

        [Test]
        public void DelayedNestedCyclicConfigurationDetected()
        {
            // Set up two states and substates, then join them
            var sm = new StateMachine<State, Trigger>(State.A);
            sm.Configure(State.B).SubstateOf(State.A);

            sm.Configure(State.C);
            sm.Configure(State.A).SubstateOf(State.C);

            Assert.Throws(typeof(ArgumentException), () => { sm.Configure(State.C).SubstateOf(State.B); });
        }
        */
    }
}
