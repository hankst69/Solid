using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Moq;
using Solid.Infrastructure.Diagnostics;

namespace Solid.TestInfrastructure.ParameterConditionTesting
{
    public class ParameterMocker : IParameterMocker
    {
        private readonly IDictionary<Type, object> m_TypeToMockedObjectDictionary = new Dictionary<Type, object>();
        private readonly KeyValuePair<Type, object> m_EmptyKeyValuePair = default;

        public void AddMockedObject(object mock)
        {
            ConsistencyCheck.EnsureArgument(mock).IsNotNull();

            var type = mock.GetType();
            if (m_TypeToMockedObjectDictionary.ContainsKey(type))
            {
                var errorMessage = ErrorMessageCreator.CreateMessageForAddMockedObjectsTypeAlreadyRegistered(type, mock);
                throw new ArgumentException(errorMessage);
            }

            m_TypeToMockedObjectDictionary.Add(type, mock);
        }

        public object CreateMockedObjectFor(Type typeToMock)
        {
            ConsistencyCheck.EnsureArgument(typeToMock).IsNotNull();

            var result = m_TypeToMockedObjectDictionary.FirstOrDefault(x => typeToMock.IsAssignableFrom(x.Key));
            if (!result.Equals(m_EmptyKeyValuePair))
                return result.Value;

            if (typeToMock.IsValueType)
                return Activator.CreateInstance(typeToMock);

            if (typeToMock == typeof(string))
                return "a string";

            if (typeToMock.FullName.StartsWith("System.Func"))
            {
                var funcReturnType = typeToMock.GetGenericArguments().First();
                var mockObject = CreateMockedObjectFor(funcReturnType);

                var func = Expression.Lambda(Expression.Constant(mockObject)).Compile();

                return func;
            }

            if (IsNonAbstractClass(typeToMock))
                return InvokeCtorFor(typeToMock);

            var instance = CreateMockInstance(typeToMock);

            return RetrieveObject(typeToMock, instance);
        }

        private static object RetrieveObject(Type typeToMock, Mock instance)
        {
            try
            {
                return instance.Object;
            }
            catch (Exception ex)
            {
                var message = string.Format("cannot resolve mocked object from mock for type '{0}'", typeToMock);
                throw new ArgumentException(message, ex);
            }
        }

        private static Mock CreateMockInstance(Type typeToMock)
        {
            try
            {
                var genericMockType = typeof(Mock<>).MakeGenericType(typeToMock);
                var asObject = Activator.CreateInstance(genericMockType);

                return (Mock)asObject;
            }
            catch (Exception ex)
            {
                var message = string.Format("cannot create instance for generic mock type for '{0}'", typeToMock);
                throw new ArgumentException(message, ex);
            }
        }

        private object InvokeCtorFor(Type typeToMock)
        {
            try
            {
                var ctorInfo = GetConstructorInfoWhereTypeItselfIsNotIncluded(typeToMock);
                var constructorParameterObjects = ctorInfo.GetParameters().Select(x => CreateMockedObjectFor(x.ParameterType)).ToArray();

                return ctorInfo.Invoke(constructorParameterObjects);
            }
            catch (Exception ex)
            {
                var message = string.Format("exception was thrown when invoking ctor for non-abstract class '{0}'", typeToMock);
                throw new ArgumentException(message, ex);
            }
        }

        private static ConstructorInfo GetConstructorInfoWhereTypeItselfIsNotIncluded(Type typeToMock)
        {
            var constructorInfos = typeToMock.GetConstructors();

            return constructorInfos.First(x => !x.GetParameters().Select(y => y.ParameterType).Contains(typeToMock));
        }

        private static bool IsNonAbstractClass(Type typeToMock)
        {
            return !typeToMock.IsAbstract && typeToMock.IsClass;
        }
    }
}