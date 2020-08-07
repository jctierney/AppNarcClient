// Copyright (c) WinQuire. All Rights Reserved. Licensed under the MIT License. See LICENSE in the project root for license information.
namespace AppTrackerClient.Entity
{
    /// <summary>
    /// DTO that defines the usage for a specific application.
    /// </summary>
    public class AppUsage
    {
        /// <summary>
        /// Gets or sets the name of the application being used/recorded.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the amount of time, in seconds, the application is being used.
        /// </summary>
        public int TimeUsed { get; set; }

        /// <summary>
        /// Gets or sets the environment where the application is being used - such as Mac vs. Windows.
        /// </summary>
        public AppEnvironment Environment { get; set; }
    }
}