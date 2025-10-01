using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using EightSeneca.Contracts;
using EightSeneca.Services;
using UserControl = System.Windows.Controls.UserControl;

namespace EightSeneca.ViewModels;

public class MainWindowViewModel : INotifyPropertyChanged
{
    private readonly IRegistryService _registryService;
    private readonly IWebEngineFactory _webEngineFactory;
    private readonly IMediaService _mediaService;

    public UserControl WebEngineView { get; }
    public string Video1 { get; }
    public string Video2 { get; }
    public string Video3 { get; }

    public MainWindowViewModel()
    {
        _registryService = new RegistryService();
        _webEngineFactory = new WebEngineFactory(_registryService);
        _mediaService = new MediaService(_registryService);

        WebEngineView = _webEngineFactory.Create();
        Video1 = _mediaService.GetVideoPath(1);
        Video2 = _mediaService.GetVideoPath(2);
        Video3 = _mediaService.GetVideoPath(3);
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
