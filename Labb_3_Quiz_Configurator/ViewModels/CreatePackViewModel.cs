using Labb_3_Quiz_Configurator.Models;

namespace Labb_3_Quiz_Configurator.ViewModels;

public class CreatePackViewModel : ViewModelBase
{
    private string _name = "";
    private Difficulty _difficulty = Difficulty.Medium;
    private int _timeLimitInSeconds = 30;

    public string Name
    {
        get => _name;
        set { _name = value; RaisePropertyChanged(); }
    }

    public Difficulty Difficulty
    {
        get => _difficulty;
        set { _difficulty = value; RaisePropertyChanged(); }
    }

    public int TimeLimitInSeconds
    {
        get => _timeLimitInSeconds;
        set { _timeLimitInSeconds = value; RaisePropertyChanged(); }
    }

}
