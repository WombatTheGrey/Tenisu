using System.Reflection;

namespace Tenisu.WebApi
{
    public static class AppVersion
    {
        public static string Value { get; }

        static AppVersion()
        {
            Value = typeof(AppVersion).Assembly
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "Unknown";
        }
    }
}
