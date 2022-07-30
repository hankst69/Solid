//----------------------------------------------------------------------------------
// <copyright file="BootStrapper.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2017-2022. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
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
        private readonly IDiContainer m_DiContainer;

        public BootStrapper(IDiContainer diContainer)
        {
            ConsistencyCheck.EnsureArgument(diContainer).IsNotNull();

            m_DiContainer = diContainer;
        }

        public void Startup(IEnumerable<IRegistrar> registrars)
        {
            ConsistencyCheck.EnsureArgument(registrars).IsNotNull();

            // run registrars
            registrars.ForEach(x => x.Register(m_DiContainer));

            // instanciate all bootables
            m_DiContainer.ResolveAllImplementing<IBootable>();
        }

        public void Shutdown()
        {
            // fini all bootables
            m_DiContainer.ResolveAllImplementing<IBootable>().Reverse().ForEach(o => o.Fini());
        }
    }
}
