// Copyright (c) WinQuire. All Rights Reserved. Licensed under the MIT License. See LICENSE in the project root for license information.
namespace AppTrackerClient.Entity
{
    /// <summary>
    /// Enum indicating the type of Environment an application is running on - for instance, Windows, Linux, or Mac.
    /// Because we are only concerned with the general environment, this is kept more abstract and does not need to be
    /// broken down to versions/distros.
    /// </summary>
    public enum AppEnvironment
    {
        /// <summary>
        /// Indicates the environment is Windows.
        /// For environment tracking, we only care that we are using Windows, not the specific version of Windows.
        /// </summary>
        Windows = 0,

        /// <summary>
        /// Indicates the environment is Linux.
        /// We only care that we are using a Linux environment - not necessarily the distro.
        /// </summary>
        Linux = 1,

        /// <summary>
        /// Indicates the environment is MacOS.
        /// </summary>
        Mac = 2,
    }
}
