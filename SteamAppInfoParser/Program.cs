using System;

namespace SteamAppInfoParser
{
    class Program
    {
        static void Main()
        {
            var appInfo = new AppInfo();
            appInfo.Read("P:/Steam/appcache/appinfo.vdf");

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
            packageInfo.Read("P:/Steam/appcache/packageinfo.vdf");

            Console.WriteLine($"{packageInfo.Packages.Count} packages");

            foreach (var package in packageInfo.Packages)
            {
                if (package.Token > 0)
                {
                    Console.WriteLine($"Package: {package.SubID} - Token: {package.Token}");
                }
            }

            Console.ReadKey();
        }
    }
}
