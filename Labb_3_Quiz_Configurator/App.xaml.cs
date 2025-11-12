using Labb_3_Quiz_Configurator.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace Labb_3_Quiz_Configurator;

public partial class App : Application
{
    protected override void OnExit(ExitEventArgs e)
    {
        var window = Application.Current?.MainWindow;
        if (window?.DataContext is MainWindowViewModel vm)
        {
            System.Diagnostics.Debug.WriteLine(">>> OnExit triggered");

            try
            {
                var focused = Keyboard.FocusedElement as UIElement;
                if (focused != null)
                {
                    focused.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                }
                else
                {
                    window.Focus();
                    var scope = FocusManager.GetFocusScope(window);
                    FocusManager.SetFocusedElement(scope, window);
                }

                vm.LoadTask?.GetAwaiter().GetResult();
                vm.SavePacksAsync().GetAwaiter().GetResult();

                System.Diagnostics.Debug.WriteLine(">>> Save completed");
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(">>> Save failed: " + ex);
            }
        }

        base.OnExit(e);
    }
}
