using System.Windows;

namespace EightSeneca.Contracts;

public interface IMediaService : IDisposable
{
    FrameworkElement GetMediaView(int slot); // 1..3
    void PlayAllFromRegistry();
}
