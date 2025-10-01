using System;
using System.Threading.Tasks;

namespace EightSeneca.WpfApp.Services
{
    /// <summary>
    /// Interface for web browser services to support multiple engines
    /// </summary>
    public interface IWebBrowserService : IDisposable
    {
        /// <summary>
        /// Gets the browser control for WPF integration
        /// </summary>
        object BrowserControl { get; }

        /// <summary>
        /// Navigates to the specified URL
        /// </summary>
        void Navigate(string url);

        /// <summary>
        /// Gets or sets whether zoom is enabled
        /// </summary>
        bool EnableZoom { get; set; }

        /// <summary>
        /// Gets or sets whether touch events are enabled
        /// </summary>
        bool EnableTouch { get; set; }

        /// <summary>
        /// Gets or sets whether external links are allowed
        /// </summary>
        bool AllowExternalLinks { get; set; }

        /// <summary>
        /// Initializes the web browser control asynchronously
        /// </summary>
        Task InitializeAsync();
    }

    /// <summary>
    /// Event arguments for external navigation events
    /// </summary>
    public class ExternalNavigationEventArgs : EventArgs
    {
        public string Url { get; set; }
        public bool Cancel { get; set; }
    }
}