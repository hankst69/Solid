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
        /// <summary>when set to true a common base folder named 'Solid.Infrastructure' is created in system Temp and system App data folders for all Solid applications</summary>
        /// <remarks>default: false</remarks>
        bool UseCommonSolidBaseFolderForAllApps { get; set; }

        /// <summary>when set to false Caches and Traces get stored under the applications temp folder - otherwise in the users application data folder</summary>
        /// <remarks>default: false</remarks>
        bool PlaceCachesAndTracesInAppDataFolder { get; set; }

        /// <summary>when set to true all temp folders that were created via GetNewEmptyTempFolder() get deleted on Dispose</summary>
        /// <remarks>default: true</remarks>
        bool DeleteCreatedTempFoldersOnDispose { get; set; }

        string GetSystemTempFolder();
        string GetSystemApplicationDataFolder();

        string GetAppDataFolder(string subFolder = null);
        string GetAppTempFolder(string subFolder = null);
        string GetAppCacheFolder(string subFolder = null);
        string GetAppTraceFolder(string subFolder = null);

        string GetNewAppTraceFile();

        string GetNewEmptyTempFolder();
        string[] GetCreatedTempFolders();
        public void DeleteCreatedTempFolders();

        // maybe move these functions into an own unit/interface
        string ConvertPathNameIntoFileName(string pathName);
        string EnsureValidPathName(string pathName);
        string EnsureValidFileName(string fileName);
    }
}
