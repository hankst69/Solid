//----------------------------------------------------------------------------------
// File: "IDiContainer.cs"
// Author: Steffen Hanke
// Date: 2017-2023
//----------------------------------------------------------------------------------
using System;

namespace Solid.Infrastructure.DiContainer
{
    /// <summary>
    /// IDiContainer
    /// </summary>
    public interface IDiContainer : IDisposable, IDiRegister, IDiResolve, IDiIsRegistered, IDiContainerSpecial
    {
    }
}