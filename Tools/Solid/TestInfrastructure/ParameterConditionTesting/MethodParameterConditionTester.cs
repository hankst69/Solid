using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using NUnit.Framework;

using Solid.Infrastructure.RuntimeTypeExtensions;

namespace Solid.TestInfrastructure.ParameterConditionTesting
{
    public class MethodParameterConditionTester
    {
        private readonly Func<object[], object> m_InvokeFunc;

        private readonly ParameterInfo[] m_ParameterInfos;
        private readonly IList<object> m_MockedCtorParameters;
        private readonly IList<string> m_NameOfParametersToSkip;

        public MethodParameterConditionTester(MethodBase methodInfo, Func<object[], object> invokeFunc, IParameterMocker parameterMocker)
        {
            m_InvokeFunc = invokeFunc;

            m_ParameterInfos = methodInfo.GetParameters();
            Name = methodInfo.Name;
            m_MockedCtorParameters = m_ParameterInfos
                .Select(x => parameterMocker.CreateMockedObjectFor(x.ParameterType))
                .ToList();
            m_NameOfParametersToSkip = new List<string>();
        }

        public string Name { get; private set; }

        public object InvokeMethod()
        {
            try
            {
                return m_InvokeFunc(m_MockedCtorParameters.ToArray());
            }
            catch (Exception e)
            {
                throw new Exception("cannot create instance of DUT, because of ", e.InnerException);
            }
        }

        public void Verify()
        {
            foreach (var index in Enumerable.Range(0, m_ParameterInfos.Length))
            {
                var currentParameterInfo = m_ParameterInfos[index];
                if (!IsParameterCheckedAgainstNull(currentParameterInfo))
                    continue;

                var parametersCopied = m_MockedCtorParameters.ToArray();
                parametersCopied[index] = null;

                try
                {
                    m_InvokeFunc(parametersCopied);

                    var errorMessage = ErrorMessageCreator.CreateMessageForMissingArgumentNullExceptionFor(index, currentParameterInfo.Name, Name);

                    throw new AssertionException(errorMessage);
                }
                catch (TargetInvocationException ex)
                {
                    var errorMessage = ErrorMessageCreator.CreateMessageForMissingArgumentNullExceptionButFoundOther(index, currentParameterInfo.Name, Name, ex);

                    var innerException = ex.InnerException;
                    innerException.Should().BeOfType<ArgumentNullException>(errorMessage);
                    innerException.Message.Should().StartWith("Value cannot be null");
                    innerException.Message.Should().EndWith($"(Parameter '{currentParameterInfo.Name}')");
                }
            }
        }

        private bool IsParameterCheckedAgainstNull(ParameterInfo parameterInfo)
        {
            if (m_NameOfParametersToSkip.Contains(parameterInfo.Name))
                return false;

            var isReferenceType = !parameterInfo.ParameterType.IsValueType;
            return isReferenceType;
        }

        public void AddParametersToSkip(IList<string> parametersToSkip)
        {
            var allSkippedParametersNotBelongingToCtor = parametersToSkip
                .Where(x => !m_ParameterInfos.Select(p => p.Name).Contains(x))
                .ToList();
            if (allSkippedParametersNotBelongingToCtor.Any())
            {
                var skippedParameterNotBelongingToCtor = allSkippedParametersNotBelongingToCtor.First();
                var message = ErrorMessageCreator.CreateMessageForSkippedParametersNotBelongingTo(Name, skippedParameterNotBelongingToCtor);

                throw new AssertionException(message);
            }

            m_NameOfParametersToSkip.AddRange(parametersToSkip);
        }
    }
}