//----------------------------------------------------------------------------------
// File: "ConsoleHelper.cs"
// Author: Steffen Hanke
// Date: 2020
//----------------------------------------------------------------------------------
using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

// ReSharper disable InconsistentNaming

namespace Solid.Infrastructure.Diagnostics
{
    public static class ConsoleHelper
    {
        public static void CreateConsole()
        {
            if (!AttachConsole(ATTACH_PARENT_PROCESS))
            {
                AllocConsole();
            }

            // reset stdout (if redirected to something else than console stdout)
            var stdoutFile = CreateFileW("CONOUT$", GENERIC_WRITE | GENERIC_READ, FILE_SHARE_WRITE, IntPtr.Zero, OPEN_EXISTING, /*FILE_ATTRIBUTE_NORMAL*/0, IntPtr.Zero);
            var currentStdout = GetStdHandle(STD_OUTPUT_HANDLE);
            if (currentStdout != stdoutFile)
            {
                SetStdHandle(STD_OUTPUT_HANDLE, stdoutFile);
            }

            // reopen stdout
            var stdoutHandle = new SafeFileHandle(stdoutFile, true);
            var stdoutFileStream = new FileStream(stdoutHandle, FileAccess.Write);
            var stdoutWriter = new StreamWriter(stdoutFileStream) { AutoFlush = true };
            Console.SetOut(stdoutWriter);

            // activate virtual terminal input mode
            if (GetConsoleMode(stdoutFile, out var cMode))
            {
                SetConsoleMode(stdoutFile, cMode | ENABLE_VIRTUAL_TERMINAL_INPUT);
            }
        }

        // WinAPI P/Invoke required:
        [DllImport("kernel32.dll")]
        private static extern bool AttachConsole(UInt32 dwProcessId);
        private const UInt32 ATTACH_PARENT_PROCESS = 0xFFFFFFFF; //define ATTACH_PARENT_PROCESS ((DWORD)-1)

        [DllImport("kernel32")]
        static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetStdHandle(UInt32 nStdHandle);
        private const UInt32 STD_OUTPUT_HANDLE = 0xFFFFFFF5; //#define STD_OUTPUT_HANDLE   ((DWORD)-11)

        [DllImport("kernel32.dll")]
        private static extern void SetStdHandle(UInt32 nStdHandle, IntPtr handle);


        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern IntPtr CreateFileW(string lpFileName, UInt32 dwDesiredAccess,
            UInt32 dwShareMode, IntPtr lpSecurityAttributes, UInt32 dwCreationDisposition,
            UInt32 dwFlagsAndAttributes, IntPtr hTemplateFile);
        private const uint GENERIC_READ = 0x80000000;
        private const uint GENERIC_WRITE = 0x40000000;
        private const uint FILE_SHARE_WRITE = 0x00000002;
        private const uint OPEN_EXISTING = 3;

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint lpMode);
        private const uint ENABLE_VIRTUAL_TERMINAL_INPUT = 0x0200;
    }
}