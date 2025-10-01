using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using WpfApp.Models;

namespace WpfApp.ViewModels;

public class MethodDialogViewModel : INotifyPropertyChanged
{
    public string Title { get; }
    public ObservableCollection<string> Options { get; }

    private string? selectedOption;
    public string? SelectedOption
    {
        get => selectedOption;
        set { selectedOption = value; OnPropertyChanged(); }
    }

    public MethodDialogViewModel(MethodOption option)
    {
        Title = option.Title;
        Options = new ObservableCollection<string>(option.Options);
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
