using System.Collections.Generic;

namespace AppTrackerClient.Entity
{
    /// <summary>
    /// Defines a user's app usage.
    /// </summary>
    public class UserAppUsage
    {
        public string UserName { get; set; }

        public List<AppUsage> AppUsages { get; set; }
    }
}
