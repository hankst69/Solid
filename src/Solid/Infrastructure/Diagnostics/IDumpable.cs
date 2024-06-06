//----------------------------------------------------------------------------------
// File: "IDumpable.cs"
// Author: Steffen Hanke
// Date: 2017-2018
//----------------------------------------------------------------------------------

namespace Solid.Infrastructure.Diagnostics
{
    /// <summary>
    /// ILogger
    /// </summary>
    public interface IDumpable
    {
        object Dump();
    }
}
