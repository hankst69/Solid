//----------------------------------------------------------------------------------
// File: "InfrastructureDiContainer.cs"
// Author: Steffen Hanke
// Date: 2022-2023
//----------------------------------------------------------------------------------

namespace Solid.Infrastructure
{
    public class InfrastructureDiContainer : DiContainer.Impl.DiContainer
    {
        public InfrastructureDiContainer()
        {
            this.Register(new InfrastructureRegistrar());
        }
    }
}
