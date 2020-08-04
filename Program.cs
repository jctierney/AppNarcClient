// Copyright (c) WinQuire. All Rights Reserved. Licensed under the MIT License. See LICENSE in the project root for license information.
namespace AppTrackerClient
{
    /// <summary>
    /// The main class that holds the main thread / entry point to the program.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main entry point of the application.
        /// </summary>
        private static void Main()
        {
            AppTracker appTracker = new AppTracker();
            appTracker.StartTracking();
        }
    }
}
