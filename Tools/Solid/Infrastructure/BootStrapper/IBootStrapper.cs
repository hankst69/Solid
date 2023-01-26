//----------------------------------------------------------------------------------
// <copyright file="IBootStrapper.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2017-2022. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
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
