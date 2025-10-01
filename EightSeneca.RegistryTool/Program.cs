using System;

namespace EightSeneca.RegistryTool
{
    class Program
    {
        static void Main(string[] args)
        {
            var registryService = new RegistryService();

            if (args.Length == 0)
            {
                PrintUsage();
                return;
            }

            string command = args[0].ToLower();

            switch (command)
            {
                case "create":
                    registryService.CreateKeys();
                    break;

                case "list":
                    registryService.ListKeys();
                    break;

                case "set":
                    if (args.Length >= 3)
                        registryService.SetValue(args[1], args[2]);
                    else
                        Console.WriteLine("Usage: set <key> <value>");
                    break;

                case "reset":
                    if (args.Length >= 2)
                        registryService.ResetValue(args[1]);
                    else
                        Console.WriteLine("Usage: reset <key>");
                    break;

                case "wipe":
                    registryService.WipeKeys();
                    break;

                default:
                    Console.WriteLine($"Unknown command: {command}");
                    PrintUsage();
                    break;
            }
        }

        static void PrintUsage()
        {
            Console.WriteLine("EightSeneca Registry Tool - Simple Version");
            Console.WriteLine("==========================================");
            Console.WriteLine("Commands:");
            Console.WriteLine("  create                    - Create registry keys");
            Console.WriteLine("  list                      - List all keys");
            Console.WriteLine("  set <key> <value>         - Set key value");
            Console.WriteLine("  reset <key>               - Reset key to default");
            Console.WriteLine("  wipe                      - Remove all keys");
            Console.WriteLine("");
            Console.WriteLine("Available keys: BrowserUrl, WebEngine, EnableZoom, EnableTouch,");
            Console.WriteLine("                AllowExternalLinks, Video1Path, Video2Path, Video3Path");
            Console.WriteLine("");
            Console.WriteLine("Examples:");
            Console.WriteLine("  EightSeneca.RegistryTool create");
            Console.WriteLine("  EightSeneca.RegistryTool set BrowserUrl https://bing.com");
            Console.WriteLine("  EightSeneca.RegistryTool set WebEngine CefSharp");
            Console.WriteLine("  EightSeneca.RegistryTool set EnableZoom false");
        }
    }
}