using Labb_3_Quiz_Configurator.Command;
namespace Labb_3_Quiz_Configurator.ViewModels;

class ResultViewModel : ViewModelBase
{
    private readonly MainWindowViewModel? _mainWindow;

    public string ResultText { get; }

    public DelegateCommand PlayAgainCommand { get; }
    public DelegateCommand BackToEditCommand { get; }

    public ResultViewModel(MainWindowViewModel? main, int score, int total)
    {
        _mainWindow = main;

        ResultText = $"You scored {score} of {total}!";

        PlayAgainCommand = new DelegateCommand(_ => PlayAgain());
        BackToEditCommand = new DelegateCommand(_ => Back());
    }

    private void PlayAgain()
    {
        _mainWindow.PlayerViewModel.StartQuiz();
        _mainWindow.ShowPlayerViewCommand.Execute(null);
    }

    private void Back()
    {
        _mainWindow.ShowConfigurationViewCommand.Execute(null);
    }
}
