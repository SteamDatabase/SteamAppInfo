using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace SteamAppInfoParser
{
    class Program
    {
        static int Main()
        {
            var steamLocation = GetSteamPath();

            if (steamLocation == null)
            {
                Console.Error.WriteLine("Can not find Steam");
                return 1;
            }

            var appInfo = new AppInfo();
            appInfo.Read(Path.Join(steamLocation, "appcache", "appinfo.vdf"));

            Console.WriteLine($"{appInfo.Apps.Count} apps");

            foreach (var app in appInfo.Apps)
            {
                if (app.Token > 0)
                {
                    Console.WriteLine($"App: {app.AppID} - Token: {app.Token}");
                }
            }

            Console.WriteLine();

            var packageInfo = new PackageInfo();
            packageInfo.Read(Path.Join(steamLocation, "appcache", "packageinfo.vdf"));

            Console.WriteLine($"{packageInfo.Packages.Count} packages");

            foreach (var package in packageInfo.Packages)
            {
                if (package.Token > 0)
                {
                    Console.WriteLine($"Package: {package.SubID} - Token: {package.Token}");
                }
            }

            return 0;
        }

        private static string GetSteamPath()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Valve\\Steam") ??
                          RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)
                              .OpenSubKey("SOFTWARE\\Valve\\Steam");

                if (key != null && key.GetValue("SteamPath") is string steamPath)
                {
                    return steamPath;
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                var paths = new [] {".steam", ".steam/steam", ".steam/root", ".local/share/Steam"};

                return paths
                    .Select(path => Path.Join(home, path))
                    .FirstOrDefault(steamPath => Directory.Exists(Path.Join(steamPath, "appcache")));
            }

            throw new PlatformNotSupportedException();
        }
    }
}
