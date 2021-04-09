namespace StoneAssemblies.MassAuth.Hosting
{
    using System;

    public static class HostingEnvironment
    {
        public static bool IsMarathonHosted()
        {
            return !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("MARATHON_APP_ID"));
        }
    }
}