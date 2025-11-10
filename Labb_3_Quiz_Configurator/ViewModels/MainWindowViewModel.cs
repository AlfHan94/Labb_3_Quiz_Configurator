using Labb_3_Quiz_Configurator.Command;
using Labb_3_Quiz_Configurator.Data;
using Labb_3_Quiz_Configurator.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Text.Json;
using System.IO;
using System.Threading.Tasks;
using System.Linq;

namespace Labb_3_Quiz_Configurator.ViewModels;

public class MainWindowViewModel : ViewModelBase
{

    public ObservableCollection<QuestionPackViewModel> Packs { get; } = new();
    public DelegateCommand SetActivePackCommand { get; }
    public DelegateCommand CreateNewPackCommand { get; }
    public DelegateCommand ShowConfigurationViewCommand { get; }
    public DelegateCommand ShowPlayerViewCommand { get; }
    public DelegateCommand ToggleFullscreenCommand { get; }

    private QuestionPackViewModel? _activePack;
    public QuestionPackViewModel? ActivePack
    {
        get => _activePack;
        set
        {
            _activePack = value;
            RaisePropertyChanged();
            PlayerViewModel?.RaisePropertyChanged(nameof(PlayerViewModel.ActivePack));
            ConfigurationViewModel?.RaisePropertyChanged(nameof(ConfigurationViewModel.ActivePack));
        }
    }
    private object _currentView;
    public object CurrentView
    {
        get => _currentView;
        set
        {
            _currentView = value;
            RaisePropertyChanged();
        }
    }
    private void ToggleFullscreen()
    {
        Application.Current.MainWindow.WindowStyle =
            Application.Current.MainWindow.WindowStyle == WindowStyle.None
            ? WindowStyle.SingleBorderWindow
            : WindowStyle.None;
        Application.Current.MainWindow.WindowState =
            Application.Current.MainWindow.WindowState == WindowState.Maximized
            ? WindowState.Normal
            : WindowState.Maximized;
    }
    public PlayerViewModel? PlayerViewModel { get; }
    public ConfigurationViewModel? ConfigurationViewModel { get; }
    public MainWindowViewModel()
    {
        Packs = new ObservableCollection<QuestionPackViewModel>();
        _ = LoadPacksAsync();
        PlayerViewModel = new PlayerViewModel(this);
        ConfigurationViewModel = new ConfigurationViewModel(this);
        CurrentView = ConfigurationViewModel;
        ShowConfigurationViewCommand = new DelegateCommand(_ => CurrentView = ConfigurationViewModel);
        ShowPlayerViewCommand = new DelegateCommand(_ =>
        {
            CurrentView = PlayerViewModel;
            PlayerViewModel.StartQuiz();
        });
        ToggleFullscreenCommand = new DelegateCommand(_ => ToggleFullscreen());
        SetActivePackCommand = new DelegateCommand(p => ActivePack = (QuestionPackViewModel?)p);
    }

    private async Task LoadPacksAsync()
    {
        var loaded = await QuestionPackStorage.LoadAsync();

        Packs.Clear();
        foreach (var pack in loaded)
            Packs.Add(new QuestionPackViewModel(pack));
    }
    public async Task SavePacksAsync()
    {
        var toSave = Packs.Select(p => p.Model).ToList();
        await QuestionPackStorage.SaveAsync(toSave);
    }


}
