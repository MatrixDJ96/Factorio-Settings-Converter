using System;
using System.IO;

namespace ModSettingsConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            string path;

            Console.WriteLine("Type mod-settings (dat/json) path: ");

            if (args.Length == 0)
            {
                var defaultPath = Path.Combine(
                    Environment.OSVersion.Platform == PlatformID.Win32NT ?
                        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Factorio") :
                        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".factorio"),
                    "mods", "mod-settings.dat"
                );

                Console.WriteLine("Default path: " + defaultPath);

                path = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(path))
                {
                    path = defaultPath;
                }
            }
            else
            {
                // Set path from args
                path = args[0];
            }


            try
            {
                if (!File.Exists(path))
                {
                    // If file is missing throw exception
                    throw new FileNotFoundException(path);
                }

                var dir = Path.GetDirectoryName(path);
                var extension = Path.GetExtension(path).ToLower();
                var filename = Path.GetFileNameWithoutExtension(path);

                var data = Array.Empty<byte>();
                var json = string.Empty;

                switch (extension)
                {
                    case ".json":
                        json = File.ReadAllText(path);
                        data = JsonUtil.JsonStringToData(json);
                        File.WriteAllBytes(Path.Combine(dir, filename + ".dat"), data);
                        break;
                    case ".dat":
                        data = File.ReadAllBytes(path);
                        json = JsonUtil.DataToJsonString(data);
                        File.WriteAllText(Path.Combine(dir, filename + ".json"), json);
                        break;
                    default:
                        throw new Exception("Unable to convert selected file...");
                }

                Console.WriteLine("Successfully converted!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
