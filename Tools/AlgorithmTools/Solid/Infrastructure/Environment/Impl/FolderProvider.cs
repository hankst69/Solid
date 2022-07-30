//----------------------------------------------------------------------------------
// File: "FolderProvider.cs"
// Author: Steffen Hanke
// Date: 2022
//----------------------------------------------------------------------------------
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Solid.Infrastructure.Diagnostics;

namespace Solid.Infrastructure.Environment.Impl
{
    public class FolderProvider : IFolderProvider
    {
        private readonly ITracer _tracer;
        private readonly string _folderProviderFullname;
        private const string c_DefaultTempFolder = @"c:\Temp";
        private const string c_DefaultAppDataFolder = @"c:\Temp";
        private readonly IList<string> _tempFolderTrackingList = new List<string>();

        public FolderProvider()
        {
            _folderProviderFullname = this.GetType().FullName;
            DeleteCreatedTempFoldersOnDispose = false;
        }

        public FolderProvider(ITracer tracer) : this()
        {
            ConsistencyCheck.EnsureArgument(tracer).IsNotNull();
            _tracer = tracer;
        }

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

        public string GetApplicationDataFolder()
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

        public string GetCacheFolder()
        {
            using var tracer = _tracer?.CreateScopeTracer();

            //var baseFolder = GetApplicationDataFolder();
            var baseFolder = GetSystemTempFolder();
            var privateBaseFolder = CreatePrivateSubFolder(baseFolder, _folderProviderFullname);
            var cacheFolder = CreatePrivateSubFolder(privateBaseFolder, "Caches");
            return cacheFolder;
        }

        public string GetTraceFolder()
        {
            using var tracer = _tracer?.CreateScopeTracer();

            //var baseFolder = GetApplicationDataFolder();
            var baseFolder = GetSystemTempFolder();
            var privateBaseFolder = CreatePrivateSubFolder(baseFolder, _folderProviderFullname);
            var traceFolder = CreatePrivateSubFolder(privateBaseFolder, "Traces");
            return traceFolder;
        }


        public string GetPrivateTempFolder(string privateName)
        {
            using var tracer = _tracer?.CreateScopeTracer();
            var baseFolder = GetSystemTempFolder();
            return CreatePrivateSubFolder(baseFolder, privateName);
        }

        public string GetPrivateApplicationDataFolder(string privateName)
        {
            using var tracer = _tracer?.CreateScopeTracer();
            var baseFolder = GetApplicationDataFolder();
            return CreatePrivateSubFolder(baseFolder, privateName);
        }

        public string GetPrivateCacheFolder(string privateName)
        {
            using var tracer = _tracer?.CreateScopeTracer();
            var baseFolder = GetCacheFolder();
            return CreatePrivateSubFolder(baseFolder, privateName);
        }

        public string GetPrivateTraceFolder(string privateName)
        {
            using var tracer = _tracer?.CreateScopeTracer();
            var baseFolder = GetTraceFolder();
            return CreatePrivateSubFolder(baseFolder, privateName);
        }

        private string CreatePrivateSubFolder(string baseFolderName, string privateName)
        {
            using var tracer = _tracer?.CreateScopeTracer();
            ConsistencyCheck.EnsureArgument(baseFolderName).IsNotNullOrEmpty().IsExistingDirectory();
            ConsistencyCheck.EnsureArgument(privateName).IsNotNullOrEmpty();

            // ensure privateFolderName only contains valid characters
            var invalidNameChars = Path.GetInvalidFileNameChars();
            foreach (char c in invalidNameChars)
            {
                privateName = privateName.Replace(c, '_'); //(c.ToString(), "");
            }
            var invalidPathChars = Path.GetInvalidPathChars();
            foreach (char c in invalidPathChars)
            {
                privateName = privateName.Replace(c, '_'); //(c.ToString(), "");
            }
            privateName = privateName.Replace('\\', '_').TrimStart('_');

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

            var baseTempFolder = GetPrivateTempFolder(_folderProviderFullname);
            var privateBaseTempFolder = CreatePrivateSubFolder(baseTempFolder, "Temps");

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

        public string GetNewTraceFile(string traceDomain)
        {
            using var tracer = _tracer?.CreateScopeTracer();

            var invalidNameChars = Path.GetInvalidFileNameChars();
            foreach (char c in invalidNameChars)
            {
                traceDomain = traceDomain.Replace(c, '_'); //(c.ToString(), "");
            }

            // we setup a new trace file which should relate given traceDomin and current date/time of creation
            var traceFolder = GetPrivateTraceFolder(traceDomain);//GetTraceFolder();

            var dateTimeScope = System.DateTime.Now.ToString("yyyyMMdd-HHmmss");
            var traceFileName = Path.Combine(traceFolder, $"{traceDomain}_{dateTimeScope}.trace");

            tracer?.Debug($"Returning new TraceFile '{traceFileName}'");
            return traceFileName;
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
        public bool DeleteCreatedTempFoldersOnDispose { get; set; }

        void System.IDisposable.Dispose()
        {
            using var tracer = _tracer?.CreateScopeTracer();
            if (DeleteCreatedTempFoldersOnDispose)
            {
                DeleteCreatedTempFolders();
            }
        }
    }
}
