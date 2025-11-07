using Labb_3_Quiz_Configurator.ViewModels;
using System.Windows;

namespace Labb_3_Quiz_Configurator.Dialogs
{
    public partial class CreateNewPackDialog : Window
    {
        public CreatePackViewModel ViewModel { get; }

        public CreateNewPackDialog()
        {
            InitializeComponent();
            ViewModel = new CreatePackViewModel();
            DataContext = ViewModel;
        }

        private void OnCreateClicked(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
