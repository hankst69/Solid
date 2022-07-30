//----------------------------------------------------------------------------------
// <copyright file="ResultValue.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2017-2020. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

using System;
using System.Runtime.Serialization;
using Solid.Infrastructure.Diagnostics;

namespace Solid.Infrastructure.Serialization
{
    /// <inheritdoc />
    /// <summary>
    /// API:NO
    /// ResultValue
    /// </summary>
    [DataContract]
    public class ResultValue : IResultValue
    {
        public ResultValue(string name, object value, Func<object, string> toString = null)
        {
            ConsistencyCheck.EnsureArgument(name).IsNotNullOrEmpty();
            ConsistencyCheck.EnsureArgument(value).IsNotNull();

            Name = name;
            Value = value;
            ValueType = value.GetType();

            m_ToString = toString ?? (o => o.ToString()); 
        }

        private readonly Func<object, string> m_ToString;

        [DataMember]
        public string Name { get; }

        [DataMember]
        public object Value { get; }

        [DataMember]
        public Type ValueType  { get; }

        public bool Equals(IResultValue other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return string.Equals(Name, other.Name) && Equals(Value, other.Value) && ValueType == other.ValueType;
        }

        public override string ToString()
        {
            return m_ToString(Value);
        }

        public override bool Equals(Object obj)
        {
            return Equals(obj as IResultValue);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Name != null ? Name.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ (Value != null ? Value.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ValueType != null ? ValueType.GetHashCode() : 0);
                //hashCode = (hashCode * 397) ^ (m_ToString != null ? m_ToString.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
