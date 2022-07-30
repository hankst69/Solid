using Solid.Infrastructure.RuntimeTypeExtensions;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Solid.TestInfrastructure.ParameterConditionTesting
{
    public interface IParameterCtorConditionsChecker
    {
        IParameterCtorConditionsChecker UseParameters(params object[] parametersToUse);

        IParameterApiConditionsChecker CheckCtorParameters();
        IParameterApiConditionsChecker CheckCtorParametersExcept(params string[] nameOfParametersToSkip);
    }

    public interface IParameterApiConditionsChecker : IConditionsCheckerVerify
    {
        IConfigureApiParameterConditionChecks CheckApi();
        IConfigureApiParameterConditionChecks CheckApiExcept(params string[] nameOfMethodsToSkip);
    }

    public interface IConfigureApiParameterConditionChecks : IConditionsCheckerVerify
    {
        IConfigureApiParameterConditionChecks ExcludeParametersFor(string methodName, params string[] nameOfParametersToSkip);
    }

    public interface IConditionsCheckerVerify
    {
        void Verify();
    }

    public class ParameterConditionsChecker :
        IParameterCtorConditionsChecker,
        IParameterApiConditionsChecker,
        IConfigureApiParameterConditionChecks
    {
        private readonly Type m_TypeOfDut;
        private readonly IParameterMocker m_ParameterMocker;
        private readonly IMethodInfoProvider m_MethodInfoProvider;

        private MethodParameterConditionTester m_CtorTester;
        private readonly IList<MethodParameterConditionTester> m_MethodTesters;

        private ParameterConditionsChecker(Type typeOfDut, IParameterMocker parameterMocker, IMethodInfoProvider methodInfoProvider)
        {
            m_TypeOfDut = typeOfDut;
            m_ParameterMocker = parameterMocker;
            m_MethodInfoProvider = methodInfoProvider;
            m_MethodTesters = new List<MethodParameterConditionTester>();
        }

        public static IParameterCtorConditionsChecker For<T>() where T : class
        {
            return new ParameterConditionsChecker(typeof(T), new ParameterMocker(), new MethodInfoProvider());
        }

        public IParameterCtorConditionsChecker UseParameters(params object[] parametersToUse)
        {
            foreach (var parameter in parametersToUse)
            {
                m_ParameterMocker.AddMockedObject(parameter);
            }

            return this;
        }

        public IParameterApiConditionsChecker CheckCtorParameters()
        {
            return CheckCtorParametersExcept();
        }

        public IParameterApiConditionsChecker CheckCtorParametersExcept(params string[] nameOfParametersToSkip)
        {
            var constructorInfo = m_MethodInfoProvider.GetConstructorInfoWithMostParameters(m_TypeOfDut);
            m_CtorTester = new MethodParameterConditionTester(constructorInfo, x => constructorInfo.Invoke(x), m_ParameterMocker);
            m_CtorTester.AddParametersToSkip(nameOfParametersToSkip.ToList());

            return this;
        }

        public void Verify()
        {
            m_CtorTester.Verify();
            m_MethodTesters.ForEach(x => x.Verify());
        }

        public IConfigureApiParameterConditionChecks ExcludeParametersFor(string methodName, params string[] nameOfParametersToSkip)
        {
            var correspondingTester = m_MethodTesters.FirstOrDefault(x => x.Name == methodName);
            if (correspondingTester == null)
            {
                var message = ErrorMessageCreator.CreateMessageForSkipMethodsOrParametersOfNotExistantMethods(m_TypeOfDut.Name, methodName);
                throw new ArgumentException(message);
            }

            correspondingTester.AddParametersToSkip(nameOfParametersToSkip.ToList());
            return this;
        }

        public IConfigureApiParameterConditionChecks CheckApi()
        {
            return CheckApiExcept();
        }

        public IConfigureApiParameterConditionChecks CheckApiExcept(params string[] nameOfMethodsToSkip)
        {
            var methodInfos = m_MethodInfoProvider.GetRelevantMethodInfosExcept(m_TypeOfDut, nameOfMethodsToSkip);

            var instanceOfDut = m_CtorTester.InvokeMethod();
            foreach (var methodInfo in methodInfos)
            {
                var copyDueToClosure = methodInfo;

                m_MethodTesters.Add(new MethodParameterConditionTester(copyDueToClosure, x => copyDueToClosure.Invoke(instanceOfDut, x), m_ParameterMocker));
            }

            return this;
        }
    }
}
