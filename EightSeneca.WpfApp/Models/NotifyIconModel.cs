using System.Windows.Input;

namespace EightSeneca.WpfApp.Models
{
    public class NotifyIconModel
    {
        public string ToolTipText { get; set; } = "EightSeneca App";
        public string IconSource { get; set; } = "/Assets/app.ico";
        public ICommand DoubleClickCommand { get; set; }
    }
}