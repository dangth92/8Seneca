using System.Windows;
using System.Windows.Input;

namespace EightSeneca.WpfApp.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            //// Kiểm tra nếu Alt+Q được nhấn
            //if (e.Key == Key.Q && (Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
            //{
            //    // Ẩn window
            //    this.Hide();

            //    // Gọi phương thức ẩn window từ ViewModel (nếu cần)
            //    if (DataContext is ViewModels.MainViewModel viewModel)
            //    {
            //        viewModel.HideWindow();
            //    }
            //}
        }
    }
}