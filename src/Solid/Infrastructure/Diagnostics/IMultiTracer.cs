//----------------------------------------------------------------------------------
// File: "IMultiTracer.cs"
// Author: Steffen Hanke
// Date: 2022
//----------------------------------------------------------------------------------

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
