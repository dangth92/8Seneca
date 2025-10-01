using System;

namespace EightSeneca.Contracts
{
    public interface IRegistryService : IDisposable
    {
        event Action<string, string?>? KeyChanged;
        string? GetString(string key);
        bool GetBool(string key);
        void SetString(string key, string value);
        void CreateDefaultsIfMissing();
    }
}
