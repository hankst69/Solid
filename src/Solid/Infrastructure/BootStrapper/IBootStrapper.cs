//----------------------------------------------------------------------------------
// File: "IBootStrapper.cs"
// Author: Steffen Hanke
// Date: 2017-2022
//----------------------------------------------------------------------------------
using System.Collections.Generic;
using Solid.Infrastructure.DiContainer;

namespace Solid.Infrastructure.BootStrapper
{
    public interface IBootStrapper
    {
		void Startup(IEnumerable<IDiRegistrar> registrars);
		
        void Shutdown();
    }
}
