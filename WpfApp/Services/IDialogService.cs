using WpfApp.Models;

namespace WpfApp.Services;

public interface IDialogService
{
    string? ShowDialog(MethodOption option);
}
