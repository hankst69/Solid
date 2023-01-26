//----------------------------------------------------------------------------------
// <copyright file="TransportContainer.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2017-2020. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

using System;
using System.IO;
using System.Runtime.Serialization;
using Solid.Infrastructure.Diagnostics;

namespace Solid.Infrastructure.Serialization
{
    [DataContract]
    public class TransportContainer
    {
        public const string TRANSPORT_CHANNEL_ID = "TRANSPORT_CHANNEL_ID";

        public TransportContainer(object payload)
        {
            ConsistencyCheck.EnsureArgument(payload).IsNotNull();

            Payload = payload;
        }

        public object Payload
        {
            private set
            {
                PayloadType = value.GetType();
                var dcs = new DataContractSerializer(PayloadType);
                var str = new MemoryStream();
                dcs.WriteObject(str, value);
                PayloadObjectSerialized = str.ToArray();
            }

            get
            {
                var dcs = new DataContractSerializer(PayloadType);
                var str = new MemoryStream(PayloadObjectSerialized);
                var obj = dcs.ReadObject(str);
                return obj;
            }
        }

        private Type PayloadType
        {
            set => PayloadTypeSerialized = value.AssemblyQualifiedName;
            get => Type.GetType(PayloadTypeSerialized);
        }

        [DataMember]
        private string PayloadTypeSerialized { get; set; }

        [DataMember]
        private byte[] PayloadObjectSerialized { get; set; }
    }
}
