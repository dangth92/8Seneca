using WpfApp.Models;
using WpfApp.ViewModels;
using WpfApp.Views;

namespace WpfApp.Services;

public class DialogService : IDialogService
{
    public string? ShowDialog(MethodOption option)
    {
        var vm = new MethodDialogViewModel(option);
        var view = new MethodDialog { DataContext = vm };
        return view.ShowDialog() == true ? vm.SelectedOption : null;
    }
}
