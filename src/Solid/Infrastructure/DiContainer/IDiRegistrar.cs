//----------------------------------------------------------------------------------
// File: "IDiRegistrar.cs"
// Author: Steffen Hanke
// Date: 2017-2023
//----------------------------------------------------------------------------------

namespace Solid.Infrastructure.DiContainer
{
    /// <summary>
    /// IDiRegistrar
    /// </summary>
    public interface IDiRegistrar
    {
        void Register(IDiContainer container);
    }
}
