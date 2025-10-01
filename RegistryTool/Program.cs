using RegistryTool.Services;

namespace RegistryTool;

internal class Program
{
    static void Main(string[] args)
    {
        var service = new RegistryService();

        if (args.Length == 0)
        {
            ShowUsage();
            return;
        }

        switch (args[0].ToLower())
        {
            case "create":
                service.CreateDefaults();
                break;
            case "list":
                service.List();
                break;
            case "change":
                if (args.Length < 3)
                {
                    Console.WriteLine("Usage: change <KeyName> <Value>");
                    return;
                }
                service.Change(args[1], args[2]);
                break;
            case "reset":
                if (args.Length < 2)
                {
                    Console.WriteLine("Usage: reset <KeyName>");
                    return;
                }
                service.Reset(args[1]);
                break;
            case "remove":
                service.RemoveAll();
                break;
            default:
                ShowUsage();
                break;
        }
    }

    static void ShowUsage()
    {
        Console.WriteLine("RegistryTool - Usage:");
        Console.WriteLine("  create                 Create default registry keys");
        Console.WriteLine("  list                   List all keys and values");
        Console.WriteLine("  change <Key> <Value>   Change specific key");
        Console.WriteLine("  reset <Key>            Reset key to default value");
        Console.WriteLine("  remove                 Remove all keys and group");
    }
}
