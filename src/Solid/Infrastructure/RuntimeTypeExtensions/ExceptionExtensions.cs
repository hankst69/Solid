//----------------------------------------------------------------------------------
// File: "ExceptionExtensions.cs"
// Author: Steffen Hanke
// Date: 2016-2023
//----------------------------------------------------------------------------------
using System;
using System.Reflection;

namespace Solid.Infrastructure.RuntimeTypeExtensions
{
    public static class ExceptionExtensions
    {
        public static string GetDetails(this Exception exception)
        {
            if (exception == null)
            {
                return "exception is null";
            }

            var padding = string.Empty;
            var exceptionDetails = string.Empty;
            while (exception != null)
            {
                //exception.PreserveStackTrace();

                exceptionDetails = string.Concat(
                    exceptionDetails,
                    $"\n-----\n{padding}Exception: {exception.GetType().Name}\n{padding}Source: {exception.Source}\n{padding}Message: {exception.Message}\nStackTrace: \n{exception.StackTrace}"
                );

                //padding = string.Concat(padding, "  "); //activate this line to have an increased indenting for each next inner exception
                exception = exception.InnerException;
            }
            return exceptionDetails;
        }

        private static readonly Action<Exception> s_InternalPreserveStackTrace =
            (Action<Exception>)Delegate.CreateDelegate(
                typeof(Action<Exception>),
                typeof(Exception).GetMethod(
                    "InternalPreserveStackTrace",
                    BindingFlags.Instance | BindingFlags.NonPublic));


        public static void PreserveStackTrace(this Exception exception)
        {
            if (exception == null)
            {
                return;
            }
            s_InternalPreserveStackTrace(exception);
        }


        // this implementation works without reflection but it will fail to work 
        // with private exceptions that do not have a serialization constructor (ObjectManager.DoFixups call)
        //public static void PreserveStackTrace(this Exception exception)
        //{
        //    if (exception == null)
        //    {
        //        return;
        //    }
        //    var ctx = new StreamingContext(StreamingContextStates.CrossAppDomain);
        //    var mgr = new ObjectManager(null, ctx);
        //    var si = new SerializationInfo(exception.GetType(), new FormatterConverter());
        //    exception.GetObjectData(si, ctx);
        //    mgr.RegisterObject(exception, 1, si); // prepare for SetObjectData
        //    mgr.DoFixups(); // ObjectManager calls SetObjectData
        //    // voila, exception is unmodified save for _remoteStackTraceString
        //}
    }
}
