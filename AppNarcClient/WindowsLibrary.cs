// Copyright (c) WinQuire. All Rights Reserved. Licensed under the MIT License. See LICENSE in the project root for license information.
namespace AppTrackerClient
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Text;

    /// <summary>
    /// A basic wrapper around some of the Win32 APIs.
    /// </summary>
    public class WindowsLibrary
    {
        private static readonly DateTime SystemStartup = DateTime.Now.AddTicks(-Environment.TickCount);

        /// <summary>
        /// Calculates the user's idle time.
        /// </summary>
        /// <returns>A timespan indicating the amount of time the user has been idle.</returns>
        public static TimeSpan UserIdleTime()
        {
            LASTINPUTINFO lastInputInfo = new LASTINPUTINFO
            {
                CbSize = (uint)LASTINPUTINFO.SizeOf,
            };

            GetLastInputInfo(ref lastInputInfo);

            int elapsedTicks = Environment.TickCount - (int)lastInputInfo.DwTime;

            if (elapsedTicks > 0)
            {
                return new TimeSpan(0, 0, 0, 0, elapsedTicks);
            }
            else
            {
                return new TimeSpan(0);
            }
        }

        /// <summary>
        /// Retrieves the active process of the foreground window the user is currently using.
        /// </summary>
        /// <returns>A process representing current foreground window.</returns>
        public static Process GetActiveProcess()
        {
            IntPtr handle = GetForegroundWindow();
            GetWindowThreadProcessId(handle, out int processId);
            Process activeProcess = Process.GetProcessById(processId);
            return activeProcess;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder buffer, int nChars);

        [DllImport("user32.dll")]
        private static extern int GetWindowThreadProcessId(IntPtr hWnd, out int processId);

        [DllImport("user32.dll")]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        [StructLayout(LayoutKind.Sequential)]
        private struct LASTINPUTINFO
        {
            public static readonly int SizeOf = Marshal.SizeOf(typeof(LASTINPUTINFO));

            [MarshalAs(UnmanagedType.U4)]
            public uint CbSize;
            [MarshalAs(UnmanagedType.U4)]
            public uint DwTime;
        }
    }
}