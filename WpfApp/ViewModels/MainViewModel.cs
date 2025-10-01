using System.ComponentModel;
using System.Runtime.CompilerServices;
using WpfApp.Helpers;
using WpfApp.Models;
using WpfApp.Services;

namespace WpfApp.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    private string groundWaterMethod = "Not Selected";
    private string thermalMethod = "Not Selected";

    public string GroundWaterMethod
    {
        get => groundWaterMethod;
        set { groundWaterMethod = value; OnPropertyChanged(); }
    }

    public string ThermalMethod
    {
        get => thermalMethod;
        set { thermalMethod = value; OnPropertyChanged(); }
    }

    public DelegateCommand OpenGroundWaterDialog { get; }
    public DelegateCommand OpenThermalDialog { get; }

    private readonly IDialogService dialogService;

    public MainViewModel(IDialogService dialogService)
    {
        this.dialogService = dialogService;

        OpenGroundWaterDialog = new DelegateCommand(_ =>
        {
            var option = new MethodOption("Ground Water Method:", new[]
            {
                "Static Water", "Steady FEA", "Transient FEA"
            });
            string? result = dialogService.ShowDialog(option);
            if (result != null)
                GroundWaterMethod = result;
        });

        OpenThermalDialog = new DelegateCommand(_ =>
        {
            var option = new MethodOption("Thermal Method:", new[]
            {
                "Static Temperature", "Steady Thermal FEA", "Transient Thermal FEA"
            });
            string? result = dialogService.ShowDialog(option);
            if (result != null)
                ThermalMethod = result;
        });
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
