using System;
using System.Collections.Generic;
using System.Reflection;

namespace Solid.TestInfrastructure.ParameterConditionTesting
{
    public interface IMethodInfoProvider
    {
        ConstructorInfo GetConstructorInfoWithMostParameters(Type type);
        IEnumerable<MethodInfo> GetRelevantMethodInfosExcept(Type type, IList<string> nameOfMethodsToSkip);
    }
}