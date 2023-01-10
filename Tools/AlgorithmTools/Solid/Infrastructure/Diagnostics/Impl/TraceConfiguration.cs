//----------------------------------------------------------------------------------
// <copyright file="ITraceConfiguration.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2022-2023. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

using Solid.Infrastructure.DiContainer;
using Solid.Infrastructure.Environment;
using System.IO;

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

        public void ConfigureFromCommandlineArgs(string[] commandLineArgs)
        {
            // todo: trace command line args and set TraceLevel and TraceTargets (File,Console) accordingly
            // for now we activate the default file tracing
            StartFileTracer();
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

            _multiTracer.AddTracer(_fileTracer);
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
