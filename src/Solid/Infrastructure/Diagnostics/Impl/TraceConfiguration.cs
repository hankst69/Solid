//----------------------------------------------------------------------------------
// File: "TraceConfiguration.cs"
// Author: Steffen Hanke
// Date: 2022-2023
//----------------------------------------------------------------------------------

using Solid.Infrastructure.DiContainer;
using Solid.Infrastructure.Environment;

using System;
using System.IO;
using System.Linq;

namespace Solid.Infrastructure.Diagnostics.Impl
{
    public class TraceConfiguration : ITraceConfiguration
    {
        #region TestSupport
        internal void TestApiSetupDelegate(ITraceConfiguration traceConfiguration) => _traceConfiguration = traceConfiguration;
        internal ITracer TestApiGetFileTracer() => _fileTracer;
        internal ITracer TestApiGetConsoleTracer() => _consoleTracer;
        #endregion

        private readonly IMultiTracer _multiTracer;
        private readonly IFolderProvider _folderProvider;
        private readonly IDiResolve _resolver;
        private IFileTracer _fileTracer;
        private IConsoleTracer _consoleTracer;
        private string _fileName;
        private string _folderName;
        private TraceLevel _levelGlobal;
        private TraceLevel _levelFile = TraceLevel.All;
        private TraceLevel _levelConsole = TraceLevel.Info;
        private ITraceConfiguration _traceConfiguration;

        public TraceConfiguration(IMultiTracer multiTracer, IFolderProvider folderProvider, IDiResolve resolver)
        {
            ConsistencyCheck.EnsureArgument(multiTracer).IsNotNull();
            ConsistencyCheck.EnsureArgument(folderProvider).IsNotNull();
            ConsistencyCheck.EnsureArgument(resolver).IsNotNull();
            _multiTracer = multiTracer;
            _folderProvider = folderProvider;
            _resolver = resolver;
            _traceConfiguration = this;

            ConfigureFromEnvironment();
        }

        public void ConfigureFromEnvironment()
        {
            // TraceTarget=Off|File[#filename]|Console
            // TraceLevel=Off|InOut|Info|Warning|Error|Debug|All
            // TraceLevel=File#Off|InOut|Info|Warning|Error|Debug|All
            // TraceLevel=Console#Off|InOut|Info|Warning|Error|Debug|All
            // TraceLevel=Off|All|InOut|Info|Warning|Error|Debug:File#Off|All|InOut|Info|Warning|Error|Debug:Console#Off|All|InOut|Info|Warning|Error|Debug

            var traceTargets = System.Environment.GetEnvironmentVariable(typeof(TraceTarget).Name);
            var traceLevels = System.Environment.GetEnvironmentVariable(typeof(TraceLevel).Name);

            if (string.IsNullOrEmpty(traceTargets) && string.IsNullOrEmpty(traceLevels)) 
            {
                return;
            }

            _traceConfiguration.ConfigureFromCommandlineArgs(new string[] 
            {
                $"-{typeof(TraceTarget).Name}:{traceTargets}",
                $"-{typeof(TraceLevel).Name}:{traceLevels}",
            });
        }

        public string[] ConfigureFromCommandlineArgs(string[] commandLineArgs)
        {
            ConsistencyCheck.EnsureArgument(commandLineArgs).IsNotNull();

            // todo: trace command line args and set TraceLevel and TraceTarget (File,Console) accordingly
            // --traceTarget:Off|File[#filename]|Console
            // --traceLevel:Off|InOut|Info|Warning|Error|Debug|All
            // --traceLevel:File#Off|InOut|Info|Warning|Error|Debug|All
            // --traceLevel:Console#Off|InOut|Info|Warning|Error|Debug|All
            // --traceLevel:Off|All|InOut|Info|Warning|Error|Debug:File#Off|All|InOut|Info|Warning|Error|Debug:Console#Off|All|InOut|Info|Warning|Error|Debug

            string c_traceTargetId = $"--{typeof(TraceTarget).Name.ToLower()}:";
            string c_traceLevelId = $"--{typeof(TraceLevel).Name.ToLower()}:";

            var traceTargetSelections = commandLineArgs
                .Where(x => !string.IsNullOrEmpty(x))
                .Where(x => x.ToLower().StartsWith(c_traceTargetId))
                .Select(x => x.Substring(c_traceTargetId.Length))
                .SelectMany(x => x.Split('|'))
                .ToArray();

            var traceLevelSelections = commandLineArgs
                .Where(x => !string.IsNullOrEmpty(x))
                .Select(x => x.ToLower())
                .Where(x => x.StartsWith(c_traceLevelId))
                .Select(x => x.Substring(c_traceLevelId.Length))
                .SelectMany(x => x.Split(':'))
                .ToArray();

            var traceLevelsGlobal = traceLevelSelections.Where(x => !x.Contains('#'))
                .ToArray();
            var traceLevelsConsole = traceLevelSelections.Where(x => x.StartsWith($"{TraceTarget.Console.ToString().ToLower()}#"))
                .Select(x => x.Substring(TraceTarget.Console.ToString().Length + 1)).ToArray();
            var traceLevelsFile = traceLevelSelections.Where(x => x.StartsWith($"{TraceTarget.File.ToString().ToLower()}#"))
                .Select(x => x.Substring(TraceTarget.File.ToString().Length + 1)).ToArray();

            var traceTargets = traceTargetSelections
                .Select(x => x.Contains('#') ? x.Split('#')[0] : x)
                .Select(x => Enum.TryParse<TraceTarget>(x.Trim(), ignoreCase: true, out TraceTarget target)
                    ? target
                    : TraceTarget.Off);

            var levelGlobal = !traceLevelsGlobal.Any() ? TraceLevel.Off : (TraceLevel) traceLevelsGlobal 
                .SelectMany(x => x.Split('|'))
                .Select(x => Enum.TryParse<TraceLevel>(x.Trim(), ignoreCase: true, out TraceLevel level)
                    ? level
                    : TraceLevel.Off)
                .Select(x => (int)x)
                .Aggregate((a, b) => a | b);

            var levelConsole = !traceLevelsConsole.Any() ? TraceLevel.Off : (TraceLevel) traceLevelsConsole
                .SelectMany(x => x.Split('|'))
                .Select(x => Enum.TryParse<TraceLevel>(x.Trim(), ignoreCase: true, out TraceLevel level)
                    ? level
                    : TraceLevel.Off)
                .Select(x => (int)x)
                .Aggregate((a, b) => a | b);

            var levelFile = !traceLevelsFile.Any() ? TraceLevel.Off : (TraceLevel) traceLevelsFile
                .SelectMany(x => x.Split('|'))
                .Select(x => Enum.TryParse<TraceLevel>(x.Trim(), ignoreCase: true, out TraceLevel level)
                    ? level
                    : TraceLevel.Off)
                .Select(x => (int)x).Aggregate((a, b) => a | b);

            var targetOff = traceTargets.Contains(TraceTarget.Off);
            var targetConsole = traceTargets.Contains(TraceTarget.Console);
            var targetFile = traceTargets.Contains(TraceTarget.File);

            var targetFileName = traceTargetSelections.FirstOrDefault(x =>
                x.ToLower().StartsWith($"{TraceTarget.File.ToString().ToLower()}#")
                && x.Split('#').Length > 1)
                ?.Split('#')[1];

            if (traceTargets.Any())
            {
                if (targetOff && !targetConsole)
                {
                    StopConsoleTracer();
                }
                if (targetOff && !targetFile)
                {
                    StopFileTracer();
                }
                if (targetConsole)
                {
                    StartConsoleTracer();
                }
                if (targetFile)
                {
                    var fileName = targetFileName != null ? Path.GetFileName(targetFileName) : null;
                    var folderName = fileName == null ? null : Path.GetDirectoryName(targetFileName);
                    StartFileTracer(fileName, folderName);
                }
            }

            if (traceLevelsGlobal.Any())
            {
                TraceLevel = levelGlobal;
            }
            if (traceLevelsConsole.Any())
            {
                ConsoleTraceLevel = levelConsole;
            }
            if (traceLevelsFile.Any())
            {
                FileTraceLevel = levelFile;
            }

            return commandLineArgs
                .Where(x => !x.ToLower().StartsWith(c_traceTargetId))
                .Where(x => !x.ToLower().StartsWith(c_traceLevelId))
                .ToArray();
        }

        public TraceLevel TraceLevel 
        { 
            get => _levelGlobal;
            set { _levelGlobal = value; if (_multiTracer != null) _multiTracer.TraceLevel = value; } 
        }

        public TraceLevel FileTraceLevel 
        { 
            get => _fileTracer != null ? _fileTracer.TraceLevel : TraceLevel.Off; 
            set { _levelFile = value;  if(_fileTracer != null) _fileTracer.TraceLevel = value; } 
        }

        public TraceLevel ConsoleTraceLevel
        {
            get => _consoleTracer != null ? _consoleTracer.TraceLevel : TraceLevel.Off;
            set { _levelConsole = value; if (_consoleTracer != null) _consoleTracer.TraceLevel = value; }
        }

        public void StartFileTracer(string fileName = null, string folderName = null)
        {
            if (_fileTracer != null && _fileName == fileName && _folderName == folderName) 
            {
                return;
            }
            _fileName = fileName;
            _folderName = folderName;

            StopFileTracer();
            
            if (string.IsNullOrEmpty(fileName))
            {
                _fileTracer =
                    _resolver.TryResolve<IFileTracer>() ??
                    new FileTracer();
            }
            else
            {
                folderName = string.IsNullOrEmpty(folderName) ? _folderProvider.GetAppTraceFolder() : folderName;

                //fileName = _folderProvider.ConvertPathNameIntoFileName(fileName);
                fileName = _folderProvider.EnsureValidFileName(fileName);
                folderName = _folderProvider.EnsureValidPathName(folderName);

                var filePath = Path.Combine(folderName, fileName);

                _fileTracer = new FileTracer(filePath);
            }

            // set default trace level for new tracer
            _fileTracer.TraceLevel = _levelFile;

            _multiTracer.AddTracer(_fileTracer);
        }

        public void StartConsoleTracer()
        {
            if (_consoleTracer != null)
            {
                return;
            }

            StopConsoleTracer();

            if (_consoleTracer != null)
            {
                _multiTracer.RemoveTracer(_consoleTracer);
            }

            _consoleTracer = 
                _resolver.TryResolve<IConsoleTracer>() ??
                new ConsoleTracer();

            // set default trace level for new tracer
            _consoleTracer.TraceLevel = _levelConsole;

            _multiTracer.AddTracer(_consoleTracer);
        }

        public void StopFileTracer()
        {
            if (_fileTracer == null)
            {
                return;
            }
            _multiTracer.RemoveTracer(_fileTracer);
            //if (!_fileTracerIsTransient)
            //{
            //    return;
            //}
            _fileTracer.Dispose();
            _fileTracer = null;
        }

        public void StopConsoleTracer()
        {
            if (_consoleTracer == null)
            {
                return;
            }
            _multiTracer.RemoveTracer(_consoleTracer);
            //if (!_consoleTracerIsTransient)
            //{
            //    return;
            //}
            _consoleTracer.Dispose();
            _consoleTracer = null;
        }
    }
}
