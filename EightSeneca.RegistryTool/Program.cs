using EightSeneca.RegistryTool;
using System;

namespace EightSeneca.RegistryTool
{
    class Program
    {
        static void Main(string[] args)
        {
            var svc = new RegistryService();

            if (args.Length == 0) { ShowUsage(); return; }

            var cmd = args[0].ToLowerInvariant();
            switch (cmd)
            {
                case "create":
                    svc.CreateDefaults(); break;
                case "list":
                    svc.List(); break;
                case "change":
                    if (args.Length < 3) { Console.WriteLine("Usage: change <Key> <Value>"); return; }
                    svc.Change(args[1], args[2]); break;
                case "reset":
                    if (args.Length < 2) { Console.WriteLine("Usage: reset <Key>"); return; }
                    svc.Reset(args[1]); break;
                case "remove":
                    svc.RemoveAll(); break;
                default:
                    ShowUsage(); break;
            }
        }

        static void ShowUsage()
        {
            Console.WriteLine("RegistryTool usage:");
            Console.WriteLine("  create");
            Console.WriteLine("  list");
            Console.WriteLine("  change <Key> <Value>");
            Console.WriteLine("  reset <Key>");
            Console.WriteLine("  remove");
        }
    }
}
