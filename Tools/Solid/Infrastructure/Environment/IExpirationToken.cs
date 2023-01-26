//----------------------------------------------------------------------------------
// <copyright file="IExpirationToken.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2020-2023. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

namespace Solid.Infrastructure.Environment
{
    /// <summary>
    /// IExpirationToken
    /// </summary>
    public interface IExpirationToken
    {
        /// <summary>The Instance the token represents</summary>
        object Instance { get; }

        /// <summary>The TimeOut of token in milliseconds</summary>
        long TimeOut { get; }

        /// <summary>The current LifeTime of token in milliseconds</summary>
        long LifeTime { get; }

        /// <summary>Check for token expiration</summary>
        bool IsExpired { get; }

        /// <summary>Refresh the token (reset LifeTime -> start Expiration TimeOut again)</summary>
        void Refresh();

        /// <summary>Check if token represents the given instance</summary>
        bool IsTokenOf(object instance);
    }
}
