//----------------------------------------------------------------------------------
// <copyright file="TraceConfigurationTests.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2023. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

using FluentAssertions;

using Moq;

using NUnit.Framework;

using Solid.Infrastructure.Diagnostics;
using Solid.Infrastructure.Diagnostics.Impl;
using Solid.Infrastructure.DiContainer;
using Solid.Infrastructure.Environment;

using System;
using System.Linq;

namespace Solid.Infrastructure_uTest.Diagnostics
{
    public class TraceConfigurationTests
    {
        TraceConfiguration _target;
        Mock<IMultiTracer> _multiTracerMock;
        Mock<IFolderProvider> _folderProviderMock;
        Mock<IDiResolve> _resolverMock;
        Mock<IFileTracer> _fileTracerMock;
        Mock<IConsoleTracer> _consoleTracerMock;
        readonly string c_TraceTarget = typeof(TraceTarget).Name;
        readonly string c_TraceLevel = typeof(TraceLevel).Name;

        [SetUp] 
        public void SetUp() 
        {
            _multiTracerMock= new Mock<IMultiTracer>();
            _folderProviderMock= new Mock<IFolderProvider>();
            _resolverMock = new Mock<IDiResolve>();
            _fileTracerMock = new Mock<IFileTracer>();
            _consoleTracerMock = new Mock<IConsoleTracer>();

            _fileTracerMock.SetupAllProperties();
            _consoleTracerMock.SetupAllProperties();

            //_resolverMock.Setup(x => x.Resolve<IMultiTracer>()).Returns(_multiTracerMock.Object);
            _resolverMock.Setup(x => x.TryResolve<IFileTracer>()).Returns(_fileTracerMock.Object);
            _resolverMock.Setup(x => x.TryResolve<IConsoleTracer>()).Returns(_consoleTracerMock.Object);

            System.Environment.SetEnvironmentVariable(c_TraceTarget, null, System.EnvironmentVariableTarget.Process);
            System.Environment.SetEnvironmentVariable(c_TraceLevel, null, System.EnvironmentVariableTarget.Process);

            _target = new TraceConfiguration(_multiTracerMock.Object, _folderProviderMock.Object, _resolverMock.Object);
        }

        [Test]
        public void ConfigureFromEnvironment_ShouldNotDelegate_WhenTraceTargetAndTraceLevelNull()
        {
            // Arrange
            var delegateTargetMock = new Mock<ITraceConfiguration>();
            _target.TestApiSetupDelegate(delegateTargetMock.Object);

            // Act
            _target.ConfigureFromEnvironment();

            // Assert
            delegateTargetMock.Verify(x => x.ConfigureFromCommandlineArgs(It.IsAny<string[]>()), Times.Never);
        }

        [Test]
        public void ConfigureFromEnvironment_ShouldDelegate_WhenTraceTargetHasValue() 
        {
            // Arrange
            var delegateTargetMock = new Mock<ITraceConfiguration>();
            _target.TestApiSetupDelegate(delegateTargetMock.Object);
            System.Environment.SetEnvironmentVariable(c_TraceTarget, TraceTarget.Off.ToString(), System.EnvironmentVariableTarget.Process);

            // Act
            _target.ConfigureFromEnvironment();

            // Assert
            delegateTargetMock.Verify(x => x.ConfigureFromCommandlineArgs(It.IsAny<string[]>()), Times.Once);
        }

        [Test]
        public void ConfigureFromEnvironment_ShouldDelegate_WhenLevelTargetHasValue()
        {
            // Arrange
            var delegateTargetMock = new Mock<ITraceConfiguration>();
            _target.TestApiSetupDelegate(delegateTargetMock.Object);
            System.Environment.SetEnvironmentVariable(c_TraceLevel, TraceLevel.Off.ToString(), System.EnvironmentVariableTarget.Process);

            // Act
            _target.ConfigureFromEnvironment();

            // Assert
            delegateTargetMock.Verify(x => x.ConfigureFromCommandlineArgs(It.IsAny<string[]>()), Times.Once);
        }


        // -TraceTarget:Off|File[#filename]|Console
        // -TraceLevel:Off|InOut|Info|Warning|Error|Debug|All
        // -TraceLevel:File#Off|InOut|Info|Warning|Error|Debug|All
        // -TraceLevel:Console#Off|InOut|Info|Warning|Error|Debug|All
        // -TraceLevel:Off|All|InOut|Info|Warning|Error|Debug:File#Off|All|InOut|Info|Warning|Error|Debug:Console#Off|All|InOut|Info|Warning|Error|Debug
        [TestCase("", false, false, false, TraceLevel.Off, TraceLevel.Off, TraceLevel.Off)]
        [TestCase("--traceTarget:Off", true, false, false, TraceLevel.Off, TraceLevel.Off, TraceLevel.Off)]
        [TestCase("--traceTarget:Console", false, true, false, TraceLevel.Off, TraceLevel.Info, TraceLevel.Off)]
        [TestCase("--traceTarget:File", false, false, true, TraceLevel.Off, TraceLevel.Off, TraceLevel.All)]
        [TestCase("--traceLevel:Console#Warning|Error|Debug", false, false, false, TraceLevel.Off, TraceLevel.Off, TraceLevel.Off)]
        [TestCase("--traceLevel:Console#Warning|Error|Debug --traceTarget:Console", false, true, false, TraceLevel.Off, TraceLevel.Warning | TraceLevel.Error | TraceLevel.Debug, TraceLevel.Off)]
        [TestCase("--traceLevel:File#InOut|Error", false, false, false, TraceLevel.Off, TraceLevel.Off, TraceLevel.Off)]
        [TestCase("--traceLevel:File#InOut|Error --traceTarget:File", false, false, false, TraceLevel.Off, TraceLevel.Off, TraceLevel.InOut|TraceLevel.Error)]
        [TestCase("--traceTarget:Off --traceLevel:File#InOut --traceLevel:Console#Warning", true, false, false, TraceLevel.Off, TraceLevel.Off, TraceLevel.Off)]
        public void ConfigureFromCommandlineArgs_ShouldMatchExpectations(string cmdLine, bool expectTargetOff, bool expectTargetConsole, bool expectTargetFile, TraceLevel expectLevel, TraceLevel expectConsoleLevel, TraceLevel expectFileLevel)
        {
            // Arrange
            var args = cmdLine.Split(' ');

            // Act
            _target.ConfigureFromCommandlineArgs(args);

            // Assert
            if (expectTargetOff)
            {
                _target.TestApiGetConsoleTracer().Should().BeNull();
                _target.TestApiGetFileTracer().Should().BeNull();
            }
            if (expectTargetConsole)
            {
                _target.TestApiGetConsoleTracer().Should().NotBeNull();
                _resolverMock.Verify(x => x.TryResolve<IConsoleTracer>(), Times.Once);
            }
            if (expectTargetFile)
            {
                _target.TestApiGetFileTracer().Should().NotBeNull();
                // next verify will fail when a specific filename is given for the FileTracer
                //_resolverMock.Verify(x => x.TryResolve<IFileTracer>(), Times.Once);
            }

            _target.TraceLevel.Should().Be(expectLevel);
            _target.FileTraceLevel.Should().Be(expectFileLevel);
            _target.ConsoleTraceLevel.Should().Be(expectConsoleLevel);
        }


        [Test]
        public void AggregateWithNullableInts()
        {
            // Arrange
            var data1 = new int?[] { 1, 2, 4, 8 };
            var data2 = new int?[] { 1, 2, null, 8 };
            // Act
            var result1 = data1.Aggregate((a,b) => a | b);
            var result2 = data2.Aggregate((a, b) => a | b);
            // Assert
            result1.Should().Be(15);
            //result2.Should().Be(11);
            result2.Should().Be(null);
        }

    }
}
