//----------------------------------------------------------------------------------
// File: "NullTracer.cs"
// Author: Steffen Hanke
// Date: 2019-2023
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
        public TraceLevel TraceLevel { get; set; }

        public void Dispose() {}
        public ITracer CreateBaseDomainTracer() => this;
        public ITracer CreateBaseDomainTracer(System.Type traceDomain) => this;
        public ITracer CreateSubDomainTracer(string subDomain) => this;
        public ITracer CreateScopeTracer(string scopeName) => this;
    }
}
