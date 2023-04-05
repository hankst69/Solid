using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Solid.Infrastructure.Diagnostics;

namespace Solid.TestInfrastructure.ParameterConditionTesting
{
    public class MethodInfoProvider : IMethodInfoProvider
    {
        private const BindingFlags c_BindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

        public IEnumerable<MethodInfo> GetRelevantMethodInfosExcept(Type type, IList<string> nameOfMethodsToSkip)
        {
            ConsistencyCheck.EnsureArgument(type).IsNotNull();
            ConsistencyCheck.EnsureArgument(nameOfMethodsToSkip).IsNotNull();

            var methodInfos = type
                .GetMethods(c_BindingFlags)
                .Where(x => !x.IsSpecialName) // e.g. removes events
                .ToList();

            var methodToSkipNotBelongingToClass = nameOfMethodsToSkip.FirstOrDefault(x => !methodInfos.Select(m => m.Name).Contains(x));
            if (methodToSkipNotBelongingToClass != null)
            {
                var message = ErrorMessageCreator.CreateMessageForSkipMethodsOrParametersOfNotExistantMethods(type.Name, methodToSkipNotBelongingToClass);
                throw new ArgumentException(message);
            }

            return methodInfos.Where(x => !nameOfMethodsToSkip.Contains(x.Name));
        }

        public ConstructorInfo GetConstructorInfoWithMostParameters(Type type)
        {
            ConsistencyCheck.EnsureArgument(type).IsNotNull();

            var constructorInfos = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            return constructorInfos.OrderBy(x => x.GetParameters().Length).Last();
        }
    }
}