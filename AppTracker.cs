// Copyright (c) WinQuire. All Rights Reserved. Licensed under the MIT License. See LICENSE in the project root for license information.
namespace AppTrackerClient
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text.Json;
    using System.Threading;
    using AppTrackerClient.Entity;

    /// <summary>
    /// The main class used to track application usage.
    /// </summary>
    public class AppTracker
    {
        /// <summary>
        /// The amound of time to wait until considering the user to be idle due to no activity.
        /// </summary>
        private static readonly int IdleTimeout = 60;

        /// <summary>
        /// How often we should send the information to the host.
        /// This is defined in seconds.
        /// </summary>
        private static readonly int TimeToSendInterval = 60;

        private static readonly string ApiKeyHeader = "ApiKey";

        private static readonly string ApiKeyHeaderValue = "{ApiKey}";

        private static readonly string BaseUrl = "https://localhost:32768/appusage";

        /// <summary>
        /// Initializes a new instance of the <see cref="AppTracker"/> class.
        /// </summary>
        public AppTracker()
        {
            this.AppUsages = new List<AppUsage>();
        }

        /// <summary>
        /// Gets or sets a current list of all the Application Usage information.
        /// </summary>
        private List<AppUsage> AppUsages { get; set; }

        /// <summary>
        /// Gets or sets the user's environment.
        /// </summary>
        private AppEnvironment UserEnvironment { get; set; }

        /// <summary>
        /// Gets or sets the time since the data was last sent to the host. This is defined in seconds.
        /// This is used to determine when to next send information to the host.
        /// </summary>
        private int TimeSinceLastSent { get; set; }

        /// <summary>
        /// Starts the tracking process.
        /// </summary>
        public void StartTracking()
        {
            this.UpdateUserEnvironment();
            while (true)
            {
                this.TrackAppUsage();
            }
        }

        /// <summary>
        /// Method that does the tracking of app usage on the system.
        /// </summary>
        protected void TrackAppUsage()
        {
            if (!this.IsUserIdle(IdleTimeout))
            {
                Process activeProcess = this.GetActiveProcess();
                if (activeProcess == null)
                {
                    Console.WriteLine("Nothing...");
                    return;
                }

                AppUsage appUsageToUpdate = this.AppUsages.Find(x => x.Name.Equals(activeProcess.ProcessName));
                if (appUsageToUpdate != null)
                {
                    appUsageToUpdate.TimeUsed++;
                }
                else
                {
                    AppUsage appUsage = new AppUsage
                    {
                        Name = activeProcess.ProcessName,
                        TimeUsed = 1,
                    };

                    this.AppUsages.Add(appUsage);
                }
            }

            Thread.Sleep(1000);
            this.TimeSinceLastSent += 1;
            this.CheckAndSendToHost();
        }

        /// <summary>
        /// Checks if it's ready to send to the host, sends the data to the host, and then cleans up the data afterwards.
        /// </summary>
        protected void CheckAndSendToHost()
        {
            if (this.TimeSinceLastSent >= TimeToSendInterval)
            {
                this.SendToHost();
                this.DeleteAppUsage();
                this.TimeSinceLastSent = 0;
            }
        }

        /// <summary>
        /// Updates the user environment.
        /// </summary>
        protected void UpdateUserEnvironment()
        {
            OperatingSystem system = Environment.OSVersion;
            PlatformID platformID = system.Platform;
            if (platformID.Equals(PlatformID.Win32NT))
            {
                this.UserEnvironment = AppEnvironment.Windows;
            }
            else if (platformID.Equals(PlatformID.MacOSX))
            {
                this.UserEnvironment = AppEnvironment.Mac;
            }
            else
            {
                this.UserEnvironment = AppEnvironment.Linux;
            }
        }

        /// <summary>
        /// Returns the active process - only supports Windows at the moment.
        /// </summary>
        /// <returns>A <see cref="Process"/> identifying the active process by the user.</returns>
        protected Process GetActiveProcess()
        {
            if (this.UserEnvironment.Equals(AppEnvironment.Windows))
            {
                return WindowsLibrary.GetActiveProcess();
            }
            else if (this.UserEnvironment.Equals(AppEnvironment.Mac))
            {
                // TODO: Add Mac environment here.
            }
            else if (this.UserEnvironment.Equals(AppEnvironment.Linux))
            {
                // TODO: Add Linux environment here.
            }

            return null;
        }

        /// <summary>
        /// Sends the information out to the host.
        /// </summary>
        protected void SendToHost()
        {
            Console.WriteLine("Sending to the host.");
            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
            };

            string jsonData = JsonSerializer.Serialize(this.AppUsages, jsonOptions);

            HttpContent content = new StringContent(jsonData);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            content.Headers.Add(ApiKeyHeader, ApiKeyHeaderValue);

            this.SendContentThroughHttpClient(content);
        }

        /// <summary>
        /// Sends the content using an <see cref="HttpClient"/>.
        /// </summary>
        /// <param name="content">Represents a <see cref="HttpContent"/> to send through the <see cref="HttpClient"/>.</param>
        protected void SendContentThroughHttpClient(HttpContent content)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.PostAsync(BaseUrl, content);
        }

        /// <summary>
        /// Deletes the current app usage on the client side.
        /// </summary>
        protected void DeleteAppUsage()
        {
            this.AppUsages = new List<AppUsage>();
        }

        /// <summary>
        /// Checks if the user is idle based on the provided idleTimeout param.
        /// </summary>
        /// <param name="idleTimeout">The timeout, in seconds, to determine if the user should be defined as idle.</param>
        /// <returns>True if the user has been idle for longer than the defined idleTimeout and false if the user is not idle.</returns>
        protected bool IsUserIdle(int idleTimeout)
        {
            if (WindowsLibrary.IdleTime.Seconds > idleTimeout)
            {
                return true;
            }

            return false;
        }
    }
}