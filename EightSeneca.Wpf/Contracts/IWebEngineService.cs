namespace EightSeneca.Wpf.Contracts
{
    public interface IWebEngineService
    {
        // Expose browser control để bind vào UI
        object BrowserControl { get; }

        // Nếu cần: method update URL runtime (registry watcher)
        void Navigate(string url);

        // Các method enable/disable zoom, touch, follow link outside domain
        void EnableZoom(bool enable);
        void EnableTouch(bool enable);
        void AllowExternalLinks(bool allow);
    }
}
