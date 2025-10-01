using System.Windows;
using WpfApp.Services;
using WpfApp.ViewModels;
using WpfApp.Views;

namespace WpfApp;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var dialogService = new DialogService();
        var vm = new MainViewModel(dialogService);
        var mainWindow = new MainWindow { DataContext = vm };
        mainWindow.Show();
    }
}

