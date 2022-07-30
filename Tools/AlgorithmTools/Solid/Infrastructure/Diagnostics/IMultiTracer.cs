//----------------------------------------------------------------------------------
// <copyright file="IMultiTracer.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2022. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

using System;

namespace Solid.Infrastructure.Diagnostics
{
    /// <summary>
    /// IMultiTracer
    /// </summary>
    public interface IMultiTracer : ITracer
    {
        IMultiTracer AddTracer(ITracer tracer);

        IMultiTracer RemoveTracer(ITracer tracer);

        IMultiTracer RemoveAllTracers();
    }
}
