//----------------------------------------------------------------------------------
// <copyright file="ITraceConfiguration.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2022-2023. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
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
        private readonly IMultiTracer _multiTracer;
        private readonly IFolderProvider _folderProvider;
        private readonly IDiResolve _resolver;
        private IFileTracer _fileTracer;
        private IConsoleTracer _consoleTracer;
        private string _fileName;
        private string _folderName;

        public TraceConfiguration(IMultiTracer multiTracer, IFolderProvider folderProvider, IDiResolve resolver)
        {
            ConsistencyCheck.EnsureArgument(multiTracer).IsNotNull();
            ConsistencyCheck.EnsureArgument(folderProvider).IsNotNull();
            ConsistencyCheck.EnsureArgument(resolver).IsNotNull();
            _multiTracer = multiTracer;
            _folderProvider = folderProvider;
            _resolver = resolver;
        }

        public string[] ConfigureFromCommandlineArgs(string[] commandLineArgs)
        {
            ConsistencyCheck.EnsureArgument(commandLineArgs).IsNotNull();

            // todo: trace command line args and set TraceLevel and TraceTarget (File,Console) accordingly
            // -TraceTarget:Off|File[#filename]|Console
            // -TraceLevel:Off|InOut|Info|Warning|Error|Debug|All
            // -TraceLevel:File#Off|InOut|Info|Warning|Error|Debug|All
            // -TraceLevel:Console#Off|InOut|Info|Warning|Error|Debug|All

            string c_traceTarget = $"-{typeof(TraceTarget).Name.ToLower()}:";
            string c_traceLevel = $"-{typeof(TraceLevel).Name.ToLower()}:";
            string c_consoleTraceLevel = $"-{typeof(TraceLevel).Name.ToLower()}:{TraceTarget.CONSOLE.ToString().ToLower()}#";
            string c_fileTraceLevel = $"-{typeof(TraceLevel).Name.ToLower()}:{TraceTarget.FILE.ToString().ToLower()}#";

            string c_targetOff = TraceTarget.OFF.ToString().ToLower();
            string c_targetConsole = TraceTarget.CONSOLE.ToString().ToLower();
            string c_targetFile = TraceTarget.FILE.ToString().ToLower();

            var traceTargetOptions = commandLineArgs
                .Where(x => x.ToLower().StartsWith(c_traceTarget))
                .Select(x => x.Substring(c_traceTarget.Length))
                .ToArray();

            var traceLevelOptions = commandLineArgs
                .Where(x => x.ToLower().StartsWith(c_traceLevel))
                .Select(x => x.Substring(c_traceLevel.Length))
                .ToArray();

            var consoleTraceLevelOptions = commandLineArgs
                .Where(x => x.ToLower().StartsWith(c_consoleTraceLevel))
                .Select(x => x.Substring(c_consoleTraceLevel.Length))
                .ToArray();

            var fileTraceLevelOptions = commandLineArgs
                .Where(x => x.ToLower().StartsWith(c_fileTraceLevel))
                .Select(x => x.Substring(c_fileTraceLevel.Length))
                .ToArray();

            if (traceTargetOptions.Any())
            {
                var targets = traceTargetOptions.SelectMany(x => x.Split("|")).ToArray();
                
                var off = targets.Any(x => x.Trim().ToLower() == c_targetOff);
                var console = targets.Any(x => x.Trim().ToLower() == c_targetConsole);
                var file = targets.Any(x => x.Split("#")[0].Trim().ToLower() == c_targetFile);

                if (off && !console)
                {
                    StopConsoleTracer();
                }
                if (off && !file)
                {
                    StopFileTracer();
                }
                if (console)
                {
                    StartConsoleTracer();
                }
                if (file)
                {
                    var filenName = targets.FirstOrDefault(x => 
                        x.Split("#")[0].Trim().ToLower() == c_targetFile && x.Split("#").Length > 1)
                        ?.Split("#")[1];

                    StartFileTracer(filenName);
                }
            }

            if (traceLevelOptions.Any())
            {
                var levels = traceLevelOptions
                    .SelectMany(x => x.Split("|"))
                    .Select(x => Enum.TryParse<TraceLevel>(x.Trim(), out TraceLevel level)
                          ? level
                          : TraceLevel.OFF);

                //var off = levels.All(x => x == TraceLevel.OFF);
                var level = (TraceLevel) levels.Select(x => (int)x).Aggregate((a, b) => a | b);

                TraceLevel = level;
            }

            if (consoleTraceLevelOptions.Any())
            {
                ConsoleTraceLevel = (TraceLevel)
                    consoleTraceLevelOptions
                    .SelectMany(x => x.Split("|"))
                    .Select(x => Enum.TryParse<TraceLevel>(x.Trim(), out TraceLevel level)
                          ? level
                          : TraceLevel.OFF)
                    .Select(x => (int)x)
                    .Aggregate((a, b) => a | b);
            }

            if (fileTraceLevelOptions.Any())
            {
                FileTraceLevel = (TraceLevel)
                    fileTraceLevelOptions
                    .SelectMany(x => x.Split("|"))
                    .Select(x => Enum.TryParse<TraceLevel>(x.Trim(), out TraceLevel level)
                          ? level
                          : TraceLevel.OFF)
                    .Select(x => (int)x)
                    .Aggregate((a, b) => a | b);
            }

            return commandLineArgs
                .Where(x => !x.ToLower().StartsWith(c_traceTarget))
                .Where(x => !x.ToLower().StartsWith(c_traceLevel))
                .ToArray();
        }

        public TraceLevel TraceLevel { get => _multiTracer.TraceLevel; set => _multiTracer.TraceLevel = value; }

        public TraceLevel FileTraceLevel { get => _fileTracer.TraceLevel; set => _fileTracer.TraceLevel = value; }

        public TraceLevel ConsoleTraceLevel { get => _consoleTracer.TraceLevel; set => _consoleTracer.TraceLevel = value; }


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

                fileName = _folderProvider.ConvertPathNameIntoFileName(fileName);
                fileName = _folderProvider.EnsureValidFileName(fileName);
                folderName = _folderProvider.EnsureValidPathName(folderName);

                var filePath = Path.Combine(folderName, fileName);

                _fileTracer = new FileTracer(filePath);
            }

            // set default trace level for new tracer
            _fileTracer.TraceLevel = TraceLevel.All;

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
            _consoleTracer.TraceLevel = TraceLevel.Info;

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
