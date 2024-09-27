//----------------------------------------------------------------------------------
// File: "ExpirationToken.cs"
// Author: Steffen Hanke
// Date: 2020-2023
//----------------------------------------------------------------------------------
using Solid.Infrastructure.Diagnostics;
using System;

namespace Solid.Infrastructure.Environment.Impl
{
    /// <inheritdoc/>
    /// <summary>
    /// API:NO
    /// ExpirationToken
    /// </summary>
    public class ExpirationToken : IExpirationToken
    {
        private DateTime m_TimeStamp;

        public ExpirationToken(object instance, long timeout)
        {
            ConsistencyCheck.EnsureArgument(instance).IsNotNull();
            ConsistencyCheck.EnsureArgument(timeout).IsGreaterOrEqual(1);

            Instance = instance;
            TimeOut = timeout;
            m_TimeStamp = DateTime.Now;
        }

        public object Instance { get; }

        public long TimeOut { get; }

        public long LifeTime => (long)(DateTime.Now - m_TimeStamp).TotalMilliseconds;

        public bool IsExpired => LifeTime > TimeOut;

        public void Refresh()
        {
            m_TimeStamp = DateTime.Now;
        }

        public bool IsTokenOf(object instance)
        {
            return Instance == instance;
        }
    }
}
