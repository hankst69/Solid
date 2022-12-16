//----------------------------------------------------------------------------------
// <copyright file="ITraceConfiguration.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2022. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

using Solid.Infrastructure.DiContainer;
using Solid.Infrastructure.Environment;
using Solid.Infrastructure.Environment.Impl;

using System.IO;

namespace Solid.Infrastructure.Diagnostics.Impl
{
    public class TraceConfiguration : ITraceConfiguration
    {
        private readonly IMultiTracer _multiTracer;
        private readonly IFolderProvider _folderProvider;
        private readonly IResolver _resolver;
        private IFileTracer _fileTracer;
        private IConsoleTracer _consoleTracer;

        public TraceConfiguration(IMultiTracer multiTracer, IFolderProvider folderProvider, IResolver resolver)
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

        public void StartFileTracer(string fileName = null, string folderName = null)
        {
            if (_fileTracer != null) 
            { 
                _multiTracer.RemoveTracer(_fileTracer);
            }
            
            if (string.IsNullOrEmpty(fileName))
            {
                _fileTracer =
                    _resolver.TryResolve<IFileTracer>() ??
                    new FileTracer();
            }
            else
            {
                folderName = string.IsNullOrEmpty(folderName) ? _folderProvider.GetAppTraceFolder() : _folderProvider.EnsureValidPathName(folderName);

                fileName = _folderProvider.ConvertPathNameIntoFileName(fileName);
                fileName = _folderProvider.EnsureValidFileName(fileName);

                var filePath = Path.Combine(folderName, fileName);

                _fileTracer = new FileTracer(filePath);
            }

            _multiTracer.AddTracer(_fileTracer);
        }

        public void StartConsoleTracer()
        {
            if (_consoleTracer != null)
            {
                _multiTracer.RemoveTracer(_consoleTracer);
            }

            _consoleTracer = 
                _resolver.TryResolve<IConsoleTracer>() ??
                new ConsoleTracer();

            _multiTracer.AddTracer(_fileTracer);
        }

        public void StopFileTracer() => _multiTracer.RemoveTracer(_fileTracer);

        public void StopConsoleTracer() => _multiTracer.RemoveTracer(_consoleTracer);
    }
}
