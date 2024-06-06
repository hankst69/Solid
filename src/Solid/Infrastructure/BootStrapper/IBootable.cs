//----------------------------------------------------------------------------------
// File: "IBootable.cs"
// Author: Steffen Hanke
// Date: 2017-2020
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
