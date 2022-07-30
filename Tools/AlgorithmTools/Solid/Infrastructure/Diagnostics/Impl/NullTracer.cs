//----------------------------------------------------------------------------------
// <copyright file="NullTracer.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2019-2022. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

namespace Solid.Infrastructure.Diagnostics.Impl
{
    /// <summary>
    /// NullTracer
    /// </summary>
    public class NullTracer : NullLogger, ITracer
    {
        public string TraceDomain => string.Empty;
        public string TraceScope => string.Empty;

        public void Dispose() {}
        public ITracer CreateBaseDomainTracer() => this;
        public ITracer CreateBaseDomainTracer(System.Type traceDomain) => this;
        public ITracer CreateSubDomainTracer(string subDomain) => this;
        public ITracer CreateScopeTracer(string scopeName) => this;
    }
}
