//----------------------------------------------------------------------------------
// File: "IResultValue.cs"
// Author: Steffen Hanke
// Date: 2017-2020
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
