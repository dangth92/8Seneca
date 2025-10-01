using System.Windows;

namespace WpfApp.Views
{
    /// <summary>
    /// Interaction logic for MethodDialog.xaml
    /// </summary>
    public partial class MethodDialog : Window
    {
        public MethodDialog()
        {
            InitializeComponent();
        }

        private void OnOkClicked(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
