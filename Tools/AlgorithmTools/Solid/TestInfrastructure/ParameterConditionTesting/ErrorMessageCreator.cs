using System;
using System.Reflection;

namespace Solid.TestInfrastructure.ParameterConditionTesting
{
    public static class ErrorMessageCreator
    {
        public static string CreateMessageForSkipMethodsOrParametersOfNotExistantMethods(string className, string methodName)
        {
            return string.Format("'{0}' does not have a method called '{1}'", className, methodName);
        }

        public static string CreateMessageForSkippedParametersNotBelongingTo(string methodName, string parameterName)
        {
            return string.Format("Cannot skip parameter testing for parameter '{0}' of '{1}', because no parameter with that name does exist.", methodName, parameterName);
        }

        public static string CreateMessageForMissingArgumentNullExceptionFor(int index, string parameterName, string methodName)
        {
            return string.Format("ArgumentNullException expected for {0} parameter (= {1}) of '{2}', but no exception was thrown", ToOrdinal(index + 1), parameterName, methodName);
        }

        public static string CreateMessageForMissingArgumentNullExceptionButFoundOther(int index, string parameterName, string nameOfMethod, TargetInvocationException ex)
        {
            return string.Format("ArgumentNullException expected for {0} parameter (= {1}) of {2}, but exception '{3}' was thrown", ToOrdinal(index + 1),
                parameterName,
                nameOfMethod,
                ex.Message);
        }

        public static string CreateMessageForAddMockedObjectsTypeAlreadyRegistered(Type type, object mock)
        {
            return string.Format("there is already an instance registered for type = '{0}', so '{1}' cannot get registered.", type, mock);
        }

        private static string ToOrdinal(int num)
        {
            switch (num % 10)
            {
                case 1:
                    return num + "st";
                case 2:
                    return num + "nd";
                case 3:
                    return num + "rd";
                default:
                    return num + "th";
            }
        }
    }
}