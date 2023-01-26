//----------------------------------------------------------------------------------
// <copyright file="ExceptionExtensions.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2016-2018. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

using System;
using System.Reflection;

namespace Solid.Infrastructure.RuntimeTypeExtensions
{
    public static class ExceptionExtensions
    {
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
        // with private exceptions that do not have a serializatoin constructor (ObjectManager.DoFixups call)
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
