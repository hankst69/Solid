//----------------------------------------------------------------------------------
// <copyright file="IResultValue.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2017-2020. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

using System;

namespace Solid.Infrastructure.Serialization
{
    /// <inheritdoc />
    /// <summary>
    /// IResultValue
    /// </summary>
    public interface IResultValue : IEquatable<IResultValue>
    {
        string Name  { get; }
        object Value { get; }
        Type ValueType { get; }

        string ToString();
    }
}
