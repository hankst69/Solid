//----------------------------------------------------------------------------------
// File: "IMultiThreadingHelper.cs"
// Author: Steffen Hanke
// Date: 2018-2023
//----------------------------------------------------------------------------------
using System;

namespace Solid.Infrastructure.Environment
{
    /// <summary>
    /// IMultiThreadingHelper
    /// </summary>
    public interface IMultiThreadingHelper
    {
        /// <summary>Executes the given action in current thread after a wait time of given milliseconds</summary>
        /// <remarks>The waiting does not cost cpu time. So this function allows to delay execution of code to a later moment.</remarks>
        /// <param name="action">The action to execute</param>
        /// <param name="milliseconds">The wait time in milliseconds</param>
        //void ExecuteDelayedInCurrentThread(Action action, int milliseconds);
        void ExecuteDelayed(Action action, int milliseconds);
    }
}
