//----------------------------------------------------------------------------------
// 2020-2024
// Author: Steffen Hanke
//----------------------------------------------------------------------------------
using System;
using Solid.Infrastructure.Diagnostics;
using Solid.Registrare;
using MeanSquareErrorImageCompare;


// 1) create DI-Container
// here we do no create a plain empty Diconainer bue instead one pre filled with basic infrastucture components
using var diContainer = new Solid.Infrastructure.InfrastructureDiContainer();

// 2) configure tracing based on commandline arguments (enforce writing of trace file)
//Environment.SetEnvironmentVariable("TraceTarget", "File",  EnvironmentVariableTarget.Process);
//args = diContainer.Resolve<ITraceConfiguration>().ConfigureFromCommandlineArgs(args.Union(new[] { "-TraceTarget:File" }).ToArray());
args = diContainer.Resolve<ITraceConfiguration>().ConfigureFromCommandlineArgs(args);

// 3) register required components at DiContainer
// register DICOM suport components
diContainer.Register(new DicomRegistrar());
// register FO-DICOM suport components
diContainer.Register(new FoDicomRegistrar());
// register specific aplication components
// here the MeanSquareError comparers
diContainer.RegisterType<IMeanSquareErrorImageComparer, MeanSquareErrorImageCompare.Impl.MeanSquareErrorImageComparer>();
diContainer.RegisterType<IMeanSquareErrorDicomFileComparer, MeanSquareErrorImageCompare.Impl.MeanSquareErrorDicomFileComparer>();

//Console.Clear();
Console.WriteLine();
if (args.Length < 1)
{
    Console.WriteLine("\nUsage:\n");
    Console.WriteLine("ImageComparerTool dicomFileOrDirectory1 dicomFileOrDirectory2 [--wait]");
    Console.WriteLine("                  [--traceTarget:Off|File[#filename]|Console]");
    Console.WriteLine("                  [--traceLevel:Off|All|InOut|Info|Warning|Error|Debug]");
    Console.WriteLine("                  [--traceLevel:File#Off|All|InOut|Info|Warning|Error|Debug]");
    Console.WriteLine("                  [--traceLevel:Console#Off|All|InOut|Info|Warning|Error|Debug]");
    Console.WriteLine();
    return;
}

bool waitForExit = false;
bool silent = false;
var fileName1 = string.Empty;
var fileName2 = string.Empty;
foreach (var arg in args)
{
    var trimmedArg = arg.Trim();
    if (trimmedArg.Length > 0)
    {
        if (trimmedArg.StartsWith("--wait"))
        {
            waitForExit = true;
        }
        else if (trimmedArg.StartsWith("--silent"))
        {
            silent = true;
        }
        else if (string.IsNullOrEmpty(fileName1))
        {
            fileName1 = trimmedArg;
        }
        else if (string.IsNullOrEmpty(fileName2))
        {
            fileName2 = trimmedArg;
        }
        else
        {
            Console.WriteLine("unknown argument '{0}'", arg);
        }
    }
}


//ConsistencyCheck.EnsureArgument(fileName1).IsNotNullOrEmpty();
//ConsistencyCheck.EnsureArgument(fileName2).IsNotNullOrEmpty();

// start it
var comparer = diContainer.Resolve<IMeanSquareErrorDicomFileComparer>();
var result = comparer.CompareDicomFiles(fileName1, fileName2, silent);
foreach(var line in result) { Console.WriteLine(line); }


if (waitForExit)
{
    Console.Write("\n\npress any key for exit");
    Console.ReadKey();
    Console.WriteLine();
}
