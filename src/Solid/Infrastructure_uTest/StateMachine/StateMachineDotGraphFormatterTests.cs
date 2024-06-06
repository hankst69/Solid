//----------------------------------------------------------------------------------
// File: "StateMachineDotGraphFormatterTests.cs"
// Author: Steffen Hanke
// Date: 2018-2019
//----------------------------------------------------------------------------------

using System;
using System.Reflection;
using FluentAssertions;
using Solid.Infrastructure.StateMachine.Impl;
using NUnit.Framework;

namespace Solid.Infrastructure_uTest.StateMachine
{
    public class StateMachineDotGraphFormatterTests
    {
        enum State
        {
            A, B, C
        }

        enum Trigger
        {
            X, Y, Z
        }

        bool IsTrue()
        {
            return true;
        }

        void OnEntry()
        {

        }

        void OnExit()
        {

        }

        [Test]
        public void SimpleTransition_DotGraph()
        {
            var expected = "digraph {" + System.Environment.NewLine
                         + " A -> B [label=\"X\"];" + System.Environment.NewLine
                         + "}";

            var sm = new StateMachine<State, Trigger>(State.A);

            sm.Configure(State.A)
                .Permit(Trigger.X, State.B);

            expected.Should().Be(StateMachineDotGraphFormatter.Format(sm.GetInfo()));
        }

        [Test]
        public void TwoSimpleTransitions_DotGraph()
        {
            var expected = "digraph {" + System.Environment.NewLine
                         + " A -> B [label=\"X\"];" + System.Environment.NewLine
                         + " A -> C [label=\"Y\"];" + System.Environment.NewLine
                         + "}";

            var sm = new StateMachine<State, Trigger>(State.A);

            sm.Configure(State.A)
                .Permit(Trigger.X, State.B)
                .Permit(Trigger.Y, State.C);

            expected.Should().Be(StateMachineDotGraphFormatter.Format(sm.GetInfo()));
        }

        [Test]
        public void WhenDiscriminatedByAnonymousGuard_DotGraph()
        {
            Func<bool> anonymousGuard = () => true;

            // TODO: Does the spec specify that when you have a guard function, the
            // description will be the result of TryGetMethodName()?  If not, we
            // shouldn't test for that exact value

            var expected = "digraph {" + System.Environment.NewLine
                         + " A -> B [label=\"X [" + anonymousGuard.GetMethodInfo().Name + "]\"];" + System.Environment.NewLine
                         + "}";

            var sm = new StateMachine<State, Trigger>(State.A);

            sm.Configure(State.A)
                .PermitIf(Trigger.X, State.B, anonymousGuard);

            expected.Should().Be(StateMachineDotGraphFormatter.Format(sm.GetInfo()));
        }

        [Test]
        public void WhenDiscriminatedByAnonymousGuardWithDescription_DotGraph()
        {
            Func<bool> anonymousGuard = () => true;

            var expected = "digraph {" + System.Environment.NewLine
                         + " A -> B [label=\"X [description]\"];" + System.Environment.NewLine
                         + "}";

            var sm = new StateMachine<State, Trigger>(State.A);

            sm.Configure(State.A)
                .PermitIf(Trigger.X, State.B, anonymousGuard, "description");

            expected.Should().Be(StateMachineDotGraphFormatter.Format(sm.GetInfo()));
        }

        [Test]
        public void WhenDiscriminatedByNamedDelegate_DotGraph()
        {
            var expected = "digraph {" + System.Environment.NewLine
                         + " A -> B [label=\"X [IsTrue]\"];" + System.Environment.NewLine
                         + "}";

            var sm = new StateMachine<State, Trigger>(State.A);

            sm.Configure(State.A)
                .PermitIf(Trigger.X, State.B, IsTrue);

            expected.Should().Be(StateMachineDotGraphFormatter.Format(sm.GetInfo()));
        }

        [Test]
        public void WhenDiscriminatedByNamedDelegateWithDescription_DotGraph()
        {
            var expected = "digraph {" + System.Environment.NewLine
                         + " A -> B [label=\"X [description]\"];" + System.Environment.NewLine
                         + "}";

            var sm = new StateMachine<State, Trigger>(State.A);

            sm.Configure(State.A)
                .PermitIf(Trigger.X, State.B, IsTrue, "description");

            expected.Should().Be(StateMachineDotGraphFormatter.Format(sm.GetInfo()));
        }

        [Test]
        public void OnEntryWithAnonymousActionAndDescription_DotGraph()
        {
            var expected = "digraph {" + System.Environment.NewLine
                         + "node [shape=box];" + System.Environment.NewLine
                         + " A -> \"enteredA\" [label=\"On Entry\" style=dotted];" + System.Environment.NewLine
                         + "}";

            var sm = new StateMachine<State, Trigger>(State.A);

            sm.Configure(State.A)
                .OnEntry(() => { }, "enteredA");

            expected.Should().Be(StateMachineDotGraphFormatter.Format(sm.GetInfo()));
        }

        [Test]
        public void OnEntryWithNamedDelegateActionAndDescription_DotGraph()
        {
            var expected = "digraph {" + System.Environment.NewLine
                         + "node [shape=box];" + System.Environment.NewLine
                         + " A -> \"enteredA\" [label=\"On Entry\" style=dotted];" + System.Environment.NewLine
                         + "}";

            var sm = new StateMachine<State, Trigger>(State.A);

            sm.Configure(State.A)
                .OnEntry(OnEntry, "enteredA");

            expected.Should().Be(StateMachineDotGraphFormatter.Format(sm.GetInfo()));
        }

        [Test]
        public void OnExitWithAnonymousActionAndDescription_DotGraph()
        {
            var expected = "digraph {" + System.Environment.NewLine
                         + "node [shape=box];" + System.Environment.NewLine
                         + " A -> \"exitA\" [label=\"On Exit\" style=dotted];" + System.Environment.NewLine
                         + "}";

            var sm = new StateMachine<State, Trigger>(State.A);

            sm.Configure(State.A)
                .OnExit(() => { }, "exitA");

            expected.Should().Be(StateMachineDotGraphFormatter.Format(sm.GetInfo()));
        }

        [Test]
        public void OnExitWithNamedDelegateActionAndDescription_DotGraph()
        {
            var expected = "digraph {" + System.Environment.NewLine
                         + "node [shape=box];" + System.Environment.NewLine
                         + " A -> \"exitA\" [label=\"On Exit\" style=dotted];" + System.Environment.NewLine
                         + "}";

            var sm = new StateMachine<State, Trigger>(State.A);

            sm.Configure(State.A)
                .OnExit(OnExit, "exitA");

            expected.Should().Be(StateMachineDotGraphFormatter.Format(sm.GetInfo()));
        }


        /* Tests that relate to functionality not provided by our StateMachine (but by StateLess implementation)
        [Test]
        public void DestinationStateIsDynamic_DotGraph()
        {
            // TODO: Since the spec doesn't guarantee that the destination text will have a
            // specific format, we shouldn't be writing a test that assumes a specific format.
        
            var expected = "digraph {" + System.Environment.NewLine
                         + " { node [label=\"?\"] unknownDestination_0 };" + System.Environment.NewLine
                         + " A -> unknownDestination_0 [label=\"X\"];" + System.Environment.NewLine
                         + "}";
        
            var sm = new StateMachine<State, Trigger>(State.A);
            sm.Configure(State.A)
                .PermitDynamic(Trigger.X, () => State.B);
        
            expected.ShouldBeEquivalentTo(StateMachineDotGraphFormatter.Format(sm.GetInfo()));
        }

        [Test]
        public void DestinationStateIsCalculatedBasedOnTriggerParameters_DotGraph()
        {
            var expected = "digraph {" + System.Environment.NewLine
                         + " { node [label=\"?\"] unknownDestination_0 };" + System.Environment.NewLine
                         + " A -> unknownDestination_0 [label=\"X\"];" + System.Environment.NewLine
                         + "}";
        
            var sm = new StateMachine<State, Trigger>(State.A);
            var trigger = sm.SetTriggerParameters<int>(Trigger.X);
            sm.Configure(State.A)
                .PermitDynamic(trigger, i => i == 1 ? State.B : State.C);
            expected.ShouldBeEquivalentTo(StateMachineDotGraphFormatter.Format(sm.GetInfo()));
        }
        
        [Test]
        public void TransitionWithIgnore_DotGraph()
        {
            // Ignored triggers do not appear in the graph
            var expected = "digraph {" + System.Environment.NewLine
                         + " A -> B [label=\"X\"];" + System.Environment.NewLine
                         + "}";
        
            var sm = new StateMachine<State, Trigger>(State.A);
        
            sm.Configure(State.A)
                .Ignore(Trigger.Y)
                .Permit(Trigger.X, State.B);
        
            expected.ShouldBeEquivalentTo(StateMachineDotGraphFormatter.Format(sm.GetInfo()));
        }
        */
    }
}
