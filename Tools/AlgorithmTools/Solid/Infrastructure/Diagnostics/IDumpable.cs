//----------------------------------------------------------------------------------
// <copyright file="IDumpable.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2017-2018. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

namespace Solid.Infrastructure.Diagnostics
{
    /// <summary>
    /// ILogger
    /// </summary>
    public interface IDumpable
    {
        object Dump();
    }
}
