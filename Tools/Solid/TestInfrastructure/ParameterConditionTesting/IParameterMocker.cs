using System;

namespace Solid.TestInfrastructure.ParameterConditionTesting
{
    public interface IParameterMocker
    {
        void AddMockedObject(object mock);
        object CreateMockedObjectFor(Type typeToMock);
    }
}