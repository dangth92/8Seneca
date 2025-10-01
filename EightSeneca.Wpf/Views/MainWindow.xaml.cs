using System.Windows;
using System.Windows.Input;

namespace EightSeneca.Wpf.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Hide window on Alt+Q
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Q && Keyboard.Modifiers == ModifierKeys.Alt)
            {
                this.Hide();
                e.Handled = true;
            }
        }
    }
}
