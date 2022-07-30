//----------------------------------------------------------------------------------
// File: "IFolderProvider.cs"
// Author: Steffen Hanke
// Date: 2022
//----------------------------------------------------------------------------------

using System;

namespace Solid.Infrastructure.Environment
{
    public interface IFolderProvider : IDisposable
    {
        string GetSystemTempFolder();
        string GetApplicationDataFolder();

        string GetCacheFolder();
        string GetTraceFolder();

        string GetPrivateTempFolder(string privateName);
        string GetPrivateApplicationDataFolder(string privateName);
        string GetPrivateCacheFolder(string privateName);
        string GetPrivateTraceFolder(string privateName);


        string GetNewEmptyTempFolder();
        string[] GetCreatedTempFolders();
        public void DeleteCreatedTempFolders();
        bool DeleteCreatedTempFoldersOnDispose { get; set; }

        string GetNewTraceFile(string traceDomain);
    }
}
