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
        private static readonly DateTime SystemStartup = DateTime.Now.AddMilliseconds(-Environment.TickCount);

        /// <summary>
        /// Gets the time of the last input from the user.
        /// </summary>
        public static DateTime LastInput => SystemStartup.AddMilliseconds(LastInputTicks);

        /// <summary>
        /// Gets the idle time of the user - i.e. the time between the last input and now.
        /// </summary>
        public static TimeSpan IdleTime => DateTime.Now.Subtract(LastInput);

        /// <summary>
        /// Gets the number of ticks since the last input from the user.
        /// </summary>
        protected static int LastInputTicks
        {
            get
            {
                var lastInputInfo = new LastInputInfo { CbSize = (uint)Marshal.SizeOf(typeof(LastInputInfo)) };
                GetLastInputInfo(ref lastInputInfo);
                return lastInputInfo.DwTime;
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

        [DllImport("User32.dll")]
        private static extern bool GetLastInputInfo(ref LastInputInfo inputInfo);

        private struct LastInputInfo
        {
            public readonly int DwTime;
            public uint CbSize;
        }
    }
}
