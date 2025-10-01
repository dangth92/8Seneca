// ITrayService.cs
using System;

namespace EightSeneca.Contracts;

public interface ITrayService : IDisposable
{
    event EventHandler? ShowRequested;
    event EventHandler? HideRequested;
    event EventHandler? ExitRequested;
    void Initialize();
}
