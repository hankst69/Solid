//----------------------------------------------------------------------------------
// File: "BootStrapper.cs"
// Author: Steffen Hanke
// Date: 2017-2022
//----------------------------------------------------------------------------------
using System.Collections.Generic;
using System.Linq;
using Solid.Infrastructure.Diagnostics;
using Solid.Infrastructure.DiContainer;
using Solid.Infrastructure.RuntimeTypeExtensions;

namespace Solid.Infrastructure.BootStrapper.Impl
{
    /// <summary>
    /// API:NO
    /// BootStrapper
    /// </summary>
    public class BootStrapper : IBootStrapper
    {
        private readonly IDiContainer _diContainer;
        private IList<IBootable> _bootables;

        public BootStrapper(IDiContainer diContainer)
        {
            ConsistencyCheck.EnsureArgument(diContainer).IsNotNull();

            _diContainer = diContainer;
        }

        public void Startup(IEnumerable<IDiRegistrar> registrars)
        {
            ConsistencyCheck.EnsureArgument(registrars).IsNotNull();

            // run registrars
            registrars.ForEach(x => x.Register(_diContainer));

            // instanciate all bootables
            _bootables = _diContainer.ResolveAllImplementing<IBootable>().ToIList();
        }

        public void Shutdown()
        {
            // fini all bootables
            _bootables?.Reverse().ForEach(o => o.Fini());
            _bootables?.Clear();
            _bootables = null;
        }
    }
}
