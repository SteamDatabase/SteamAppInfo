using System;
using System.IO;
using Microsoft.Win32;

namespace SteamAppInfoParser
{
    class Program
    {
        static int Main()
        {
            var packageInfo2 = new PackageInfo();
            packageInfo2.Read(@"C:\Users\xPaw\Desktop\packageinfo.vdf");
            

            string steamLocation = null;

            var key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Valve\\Steam") ??
                      RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey("SOFTWARE\\Valve\\Steam");

            if (key != null && key.GetValue("SteamPath") is string steamPath)
            {
                steamLocation = steamPath;
            }

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
    }
}
