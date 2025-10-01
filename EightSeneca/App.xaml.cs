using EightSeneca.Services;
using System.Windows;
using Application = System.Windows.Application;

namespace EightSeneca;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private TrayService? _trayService;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        _trayService = new TrayService();
        _trayService.Initialize();
    }
}

