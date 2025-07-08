using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using ValveKeyValue;

namespace SteamAppInfoParser;

class Program
{
    static int Main(string[] args)
    {
        Console.WriteLine("Usage: ./app <app/package> <path to vdf>");
        Console.WriteLine("Do not specify arguments if you want to dump both files from your Steam client.");

        if (args.Length == 2)
        {
            var type = args[0];
            var file = args[1];

            if (type != "app" && type != "package")
            {
                Console.WriteLine("Wrong type.");
            }

            if (!File.Exists(file))
            {
                Console.WriteLine($"\"{file}\" does not exist.");
                return 1;
            }

            if (type == "app")
            {
                DumpAppInfo(file);
            }

            if (type == "package")
            {
                DumpPackageInfo(file);
            }

            return 0;
        }
        else if (args.Length != 0)
        {
            return 1;
        }

        var steamLocation = GetSteamPath();

        if (steamLocation == null)
        {
            Console.Error.WriteLine("Can not find Steam");
            return 1;
        }

        DumpAppInfo(Path.Join(steamLocation, "appcache", "appinfo.vdf"));
        DumpPackageInfo(Path.Join(steamLocation, "appcache", "packageinfo.vdf"));

        return 0;
    }

    private static void DumpAppInfo(string file)
    {
        Console.WriteLine($"Reading {file}");

        var appInfo = new AppInfo();
        appInfo.Read(file);
        Console.WriteLine($"{appInfo.Apps.Count} apps");

        using var stream = File.OpenWrite("appinfo_text.vdf");

        var serializer = KVSerializer.Create(KVSerializationFormat.KeyValues1Text);

        foreach (var app in appInfo.Apps)
        {
            var kv = new KVObject($"app_{app.AppID}", app.Data.Value)
            {
                new KVObject("_token", app.Token),
                new KVObject("_changenumber", (long)app.ChangeNumber),
                new KVObject("_updated", app.LastUpdated.ToString("s")),
                new KVObject("_hash", Convert.ToHexString([.. app.Hash]))
            };
            serializer.Serialize(stream, kv);
        }

        Console.WriteLine($"Saved to {stream.Name}");
    }

    private static void DumpPackageInfo(string file)
    {
        Console.WriteLine($"Reading {file}");

        var packageInfo = new PackageInfo();
        packageInfo.Read(file);
        Console.WriteLine($"{packageInfo.Packages.Count} packages");

        using var stream = File.OpenWrite("packageinfo_text.vdf");

        var serializer = KVSerializer.Create(KVSerializationFormat.KeyValues1Text);

        foreach (var app in packageInfo.Packages)
        {
            var kv = new KVObject($"package_{app.SubID}", app.Data.Value)
            {
                new KVObject("_token", app.Token),
                new KVObject("_changenumber", (long)app.ChangeNumber),
                new KVObject("_hash", Convert.ToHexString([.. app.Hash]))
            };
            serializer.Serialize(stream, kv);
        }

        Console.WriteLine($"Saved to {stream.Name}");
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
            var paths = new[] { ".steam", ".steam/steam", ".steam/root", ".local/share/Steam" };

            return paths
                .Select(path => Path.Join(home, path))
                .FirstOrDefault(steamPath => Directory.Exists(Path.Join(steamPath, "appcache")));
        }
        else if (OperatingSystem.IsMacOS())
        {
            var home = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            return Path.Join(home, "Steam");
        }

        throw new PlatformNotSupportedException();
    }
}
