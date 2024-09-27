//----------------------------------------------------------------------------------
// File: "IDiResolve.cs"
// Author: Steffen Hanke
// Date: 2017-2023
//----------------------------------------------------------------------------------
using System.Collections.Generic;

namespace Solid.Infrastructure.DiContainer
{
    /// <summary>
    /// IDiResolve
    /// </summary>
    public interface IDiResolve
    {
        TTypeToResolve Resolve<TTypeToResolve>();

        TTypeToResolve TryResolve<TTypeToResolve>();
    }
}
