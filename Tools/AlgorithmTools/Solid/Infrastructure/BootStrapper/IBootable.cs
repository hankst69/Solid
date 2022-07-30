//----------------------------------------------------------------------------------
// <copyright file="IBootable.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2017-2020. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

namespace Solid.Infrastructure.BootStrapper
{
    public interface IBootable
    {
        /// <summary>
        /// Shutdown
        /// </summary>
        void Fini();
    }
}
