using Labb_3_Quiz_Configurator.ViewModels;
using System.Windows;

namespace Labb_3_Quiz_Configurator.Dialogs;

public partial class PackOptionsDialog : Window
{
    public PackOptionsDialog()
    {
        InitializeComponent();
        DataContext = new CreatePackViewModel();
    }

    private void Ok_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }
}
