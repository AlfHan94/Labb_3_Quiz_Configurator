using Labb_3_Quiz_Configurator.ViewModels;
using System.Windows;

namespace Labb_3_Quiz_Configurator;
public partial class MainWindow : Window
{

    public MainWindow()
    {
        InitializeComponent();

        DataContext = new MainWindowViewModel();
    }


}