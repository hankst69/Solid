//----------------------------------------------------------------------------------
// File: "StateMachineDotGraphFormatter.cs"
// Author: Steffen Hanke
// Date: 2018-2022
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;

namespace Solid.Infrastructure.StateMachine.Impl
{
    public class StateMachineDotGraphFormatter
    {
        /// <summary>
        /// Produces a DOT GraphViz graph.
        /// </summary>
        /// <param name="stateMachineInfo">The StateMachineInfo to be mapped.</param>
        /// <returns>DOT GraphViz text.</returns>
        public static string Format(IStateMachineInfo stateMachineInfo)
        {
            return Format(stateMachineInfo, new string[] {}, false);
        }

        /// <summary>
        /// Produces a DOT GraphViz graph.
        /// </summary>
        /// <param name="stateMachineInfo">The StateMachineInfo to be mapped.</param>
        /// <param name="excludedTriggers">List of Triggers (names) which is to be excluded in the visualization.</param>
        /// <param name="specialFormatting">Add some DOT formatting statements to produce a nicer looking graph.</param>
        /// <returns>DOT GraphViz text.</returns>
        public static string Format(IStateMachineInfo stateMachineInfo, IEnumerable<string> excludedTriggers, bool specialFormatting)
        {
            if (stateMachineInfo == null)
            {
                throw new ArgumentNullException("stateMachineInfo");
            }
            if (excludedTriggers == null)
            {
                throw new ArgumentNullException("excludedTriggers");
            }

            var bindings = stateMachineInfo.States.ToArray();

            var lines = new List<string>();
            var unknownDestinations = new List<string>();

            foreach (var binding in bindings)
            {
                var fixedTransitions = binding.FixedTransitions.Where(t => !excludedTriggers.Contains(t.Trigger.ToString())).ToArray();
                var dynamicTransitions = binding.DynamicTransitions.Where(t => !excludedTriggers.Contains(t.Trigger.ToString())).ToArray();

                unknownDestinations.AddRange(dynamicTransitions.Select(t => t.Destination));

                var source = binding.ToString();
                foreach (var transition in fixedTransitions)
                {
                    HandleTransitions(ref lines, source, transition.Trigger.ToString(), transition.DestinationState.ToString(), transition.GuardDescription);
                }
                
                foreach (var transition in dynamicTransitions)
                {
                    HandleTransitions(ref lines, source, transition.Trigger.ToString(), transition.Destination, transition.GuardDescription);
                }
            }

            if (unknownDestinations.Any())
            {
                string label = string.Format(" {{ node [label=\"?\"] {0} }};", string.Join(" ", unknownDestinations));
                lines.Insert(0, label);
            }

            if (bindings.Any(s => s.EntryActions.Any() || s.ExitActions.Any()))
            {
                lines.Add("node [shape=box];");

                foreach (var binding in bindings)
                {
                    var source = binding.ToString();

                    foreach (var entryActionBehaviour in binding.EntryActions)
                    {
                        string line = string.Format(" {0} -> \"{1}\" [label=\"On Entry\" style=dotted];", source, entryActionBehaviour);
                        lines.Add(line);
                    }

                    foreach (var exitActionBehaviour in binding.ExitActions)
                    {
                        string line = string.Format(" {0} -> \"{1}\" [label=\"On Exit\" style=dotted];", source, exitActionBehaviour);
                        lines.Add(line);
                    }
                }
            }

            if (specialFormatting)
            {
                return "digraph {" + System.Environment.NewLine +
                         "overlap=false;" + System.Environment.NewLine +
                         "node[shape = ellipse, style = filled];" + System.Environment.NewLine +
                         string.Join(System.Environment.NewLine, lines) + System.Environment.NewLine +
                       "}";
            }

            return "digraph {" + System.Environment.NewLine +
                     string.Join(System.Environment.NewLine, lines) + System.Environment.NewLine +
                   "}";
        }

        private static void HandleTransitions(ref List<string> lines, string sourceState, string trigger, string destination, string guardDescription)
        {
            string line = string.IsNullOrWhiteSpace(guardDescription) ?
                string.Format(" {0} -> {1} [label=\"{2}\"];", sourceState, destination, trigger) :
                string.Format(" {0} -> {1} [label=\"{2} [{3}]\"];", sourceState, destination, trigger, guardDescription);

            lines.Add(line);
        }
    }
}
