//----------------------------------------------------------------------------------
// File: "IDiIsRegistered.cs"
// Author: Steffen Hanke
// Date: 2023
//----------------------------------------------------------------------------------

namespace Solid.Infrastructure.DiContainer
{
    /// <summary>
    /// IDiIsRegistered
    /// </summary>
    public interface IDiIsRegistered
    {
        ///<summary>checks if type is registered with container</summary>
        bool IsRegistered<TTypeToResolve>();

        ///<summary>checks if any registration exists that implements the given type</summary>
        bool IsRegisteredAnyImplementing<TTypeToResolve>();
    }
}
