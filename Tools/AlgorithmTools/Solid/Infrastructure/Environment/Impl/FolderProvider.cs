//----------------------------------------------------------------------------------
// File: "FolderProvider.cs"
// Author: Steffen Hanke
// Date: 2022
//----------------------------------------------------------------------------------
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using Solid.Infrastructure.Diagnostics;

namespace Solid.Infrastructure.Environment.Impl
{
    public class FolderProvider : IFolderProvider
    {
        private readonly ITracer _tracer;
        private readonly string _solidFolderName;
        private readonly string _appFolderName;
        private const string c_DefaultTempFolder = @"c:\Temp";
        private const string c_DefaultAppDataFolder = @"c:\Temp";
        private readonly IList<string> _tempFolderTrackingList = new List<string>();

        public FolderProvider()
        {
            _solidFolderName = Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location);
            _appFolderName = Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location);
            UseCommonSolidBaseFolderForAllApps = false;
            PlaceCachesAndTracesInAppDataFolder = false;
            DeleteCreatedTempFoldersOnDispose = true;
        }

        public FolderProvider(ITracer tracer) : this()
        {
            ConsistencyCheck.EnsureArgument(tracer).IsNotNull();
            _tracer = tracer;
        }

        public bool UseCommonSolidBaseFolderForAllApps { get; set; }

        public bool PlaceCachesAndTracesInAppDataFolder { get; set; }

        public bool DeleteCreatedTempFoldersOnDispose { get; set; }

        public string GetSystemTempFolder()
        {
            using var tracer = _tracer?.CreateScopeTracer();

            var systemTempFolder = System.Environment.GetEnvironmentVariable("TEMP", System.EnvironmentVariableTarget.Machine); //.User);
            if (string.IsNullOrEmpty(systemTempFolder))
                systemTempFolder = c_DefaultTempFolder;
            if (!Directory.Exists(systemTempFolder))
                Directory.CreateDirectory(systemTempFolder);

            tracer?.Debug($"Returning system temp folder '{systemTempFolder}'");
            ConsistencyCheck.EnsureValue(systemTempFolder).IsNotNullOrEmpty().IsExistingDirectory();
            return systemTempFolder;
        }

        public string GetSystemApplicationDataFolder()
        {
            using var tracer = _tracer?.CreateScopeTracer();

            var appDataFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) ?? c_DefaultAppDataFolder;
            if (string.IsNullOrEmpty(appDataFolder))
                appDataFolder = c_DefaultAppDataFolder;
            if (!Directory.Exists(appDataFolder))
                Directory.CreateDirectory(appDataFolder);

            tracer?.Debug($"Returning system temp folder '{appDataFolder}'");
            ConsistencyCheck.EnsureValue(appDataFolder).IsNotNullOrEmpty().IsExistingDirectory();
            return appDataFolder;
        }

        public string GetAppTempFolder(string subFolder = null)
        {
            using var tracer = _tracer?.CreateScopeTracer();

            var folder = GetSystemTempFolder();

            folder = UseCommonSolidBaseFolderForAllApps ? CreatePrivateSubFolder(folder, _solidFolderName) : folder;
            folder = CreatePrivateSubFolder(folder, _appFolderName);
            folder = !string.IsNullOrEmpty(subFolder) ? CreatePrivateSubFolder(folder, subFolder) : folder;
            return folder;
        }

        public string GetAppDataFolder(string subFolder = null)
        {
            using var tracer = _tracer?.CreateScopeTracer();

            var folder = GetSystemApplicationDataFolder();

            folder = UseCommonSolidBaseFolderForAllApps ? CreatePrivateSubFolder(folder, _solidFolderName) : folder;
            folder = CreatePrivateSubFolder(folder, _appFolderName);
            folder = !string.IsNullOrEmpty(subFolder) ? CreatePrivateSubFolder(folder, subFolder) : folder;
            return folder;
        }

        public string GetAppCacheFolder(string subFolder = null)
        {
            using var tracer = _tracer?.CreateScopeTracer();

            const string cCaches = "Caches";
            var folder = PlaceCachesAndTracesInAppDataFolder ? GetAppDataFolder(cCaches) : GetAppTempFolder(cCaches);
            folder = !string.IsNullOrEmpty(subFolder) ? CreatePrivateSubFolder(folder, subFolder) : folder;
            return folder;
        }

        public string GetAppTraceFolder(string subFolder = null)
        {
            using var tracer = _tracer?.CreateScopeTracer();

            const string cTraces = "Traces";
            var folder = PlaceCachesAndTracesInAppDataFolder ? GetAppDataFolder(cTraces) : GetAppTempFolder(cTraces);
            folder = !string.IsNullOrEmpty(subFolder) ? CreatePrivateSubFolder(folder, subFolder) : folder;
            return folder;
        }

        private string CreatePrivateSubFolder(string baseFolderName, string privateName)
        {
            using var tracer = _tracer?.CreateScopeTracer();
            ConsistencyCheck.EnsureArgument(baseFolderName).IsNotNullOrEmpty().IsExistingDirectory();
            ConsistencyCheck.EnsureArgument(privateName).IsNotNullOrEmpty();

            // ensure privateFolderName only contains valid characters and expresses the name of a single folder
            privateName = EnsureValidFileName(privateName);
            privateName = EnsureValidPathName(privateName);
            privateName = ConvertPathNameIntoFileName(privateName);

            var privateFolderName = Path.Combine(baseFolderName, privateName);
            if (!Directory.Exists(privateFolderName))
                Directory.CreateDirectory(privateFolderName);

            tracer?.Debug($"Returning private folder '{privateFolderName}'");
            ConsistencyCheck.EnsureValue(privateFolderName).IsNotNullOrEmpty().IsExistingDirectory();
            return privateFolderName;
        }

        public string GetNewEmptyTempFolder()
        {
            using var tracer = _tracer?.CreateScopeTracer();

            var privateBaseTempFolder = GetAppTempFolder("Temps");

            var count = 0;
            var emptyTempFolder = Path.Combine(privateBaseTempFolder, $"_{count}");
            while (Directory.Exists(emptyTempFolder))
            {
                count++;
                emptyTempFolder = Path.Combine(privateBaseTempFolder, $"_{count}");
            }
            Directory.CreateDirectory(emptyTempFolder);
            _tempFolderTrackingList.Add(emptyTempFolder);

            tracer?.Debug($"Returning new empty temp folder '{emptyTempFolder}'");
            ConsistencyCheck.EnsureValue(emptyTempFolder).IsNotNullOrEmpty().IsExistingDirectory();
            return emptyTempFolder;
        }

        public string[] GetCreatedTempFolders()
        {
            var folders = _tempFolderTrackingList.Distinct().ToArray();
            return folders;
        }

        public void DeleteCreatedTempFolders()
        {
            using var tracer = _tracer?.CreateScopeTracer();
            foreach (var folder in _tempFolderTrackingList)
            {
                if (Directory.Exists(folder))
                {
                    tracer?.Info($"deleting temp folder '{folder}'");
                    Directory.Delete(folder, recursive: true);
                }
            }
            _tempFolderTrackingList.Clear();
        }

        void System.IDisposable.Dispose()
        {
            using var tracer = _tracer?.CreateScopeTracer();
            if (DeleteCreatedTempFoldersOnDispose)
            {
                DeleteCreatedTempFolders();
            }
        }

        public string ConvertPathNameIntoFileName(string pathName)
        {
            pathName = pathName.Replace('\\', '_').TrimStart('_');
            pathName = pathName.Replace('/', '_').TrimStart('_');
            return EnsureValidFileName(pathName);
        }

        public string EnsureValidPathName(string pathName)
        {
            var invalidPathChars = Path.GetInvalidPathChars();
            foreach (char c in invalidPathChars)
            {
                pathName = pathName.Replace(c, '_'); //(c.ToString(), "");
            }
            return pathName;
        }

        public string EnsureValidFileName(string fileName)
        {
            var invalidNameChars = Path.GetInvalidFileNameChars();
            foreach (char c in invalidNameChars)
            {
                fileName = fileName.Replace(c, '_'); //(c.ToString(), "");
            }
            return fileName;
        }

        public string GetNewAppTraceFile()
        {
            using var tracer = _tracer?.CreateScopeTracer();

            var appName = _appFolderName;
            appName = ConvertPathNameIntoFileName(appName);
            appName = EnsureValidFileName(appName);

            // we setup a new trace file which should relate given traceDomin and current date/time of creation
            var traceFolder = GetAppTraceFolder();

            var dateTimeScope = System.DateTime.Now.ToString("yyyyMMdd-HHmmss");
            var traceFileName = Path.Combine(traceFolder, $"{appName}_{dateTimeScope}.trace");

            tracer?.Debug($"Returning new TraceFile '{traceFileName}'");
            return traceFileName;
        }
    }
}
