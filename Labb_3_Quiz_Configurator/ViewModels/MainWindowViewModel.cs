using Labb_3_Quiz_Configurator.Command;
using Labb_3_Quiz_Configurator.Data;
using Labb_3_Quiz_Configurator.Models;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text.Json;
using System.Windows;
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

            ShowConfigurationViewCommand?.RaiseCanExecuteChanged();
            ShowPlayerViewCommand?.RaiseCanExecuteChanged();
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
    public Task LoadTask { get; }

    private CancellationTokenSource? _saveCts;
    private readonly object _saveLock = new();

    public MainWindowViewModel()
    {
        Packs.CollectionChanged += Packs_CollectionChanged;

        LoadTask = LoadPacksAsync();
        PlayerViewModel = new PlayerViewModel(this);
        ConfigurationViewModel = new ConfigurationViewModel(this);

        CurrentView = ConfigurationViewModel;

        ShowConfigurationViewCommand = new DelegateCommand(_ => CurrentView = ConfigurationViewModel,
                                                          _ => CurrentView != ConfigurationViewModel);
        ShowPlayerViewCommand = new DelegateCommand(_ => { CurrentView = PlayerViewModel; PlayerViewModel.StartQuiz(); },
                                                   _ => CurrentView != PlayerViewModel);

        ToggleFullscreenCommand = new DelegateCommand(_ => ToggleFullscreen());
        SetActivePackCommand = new DelegateCommand(p => ActivePack = (QuestionPackViewModel?)p);
    }

    private void Packs_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
        {
            foreach (QuestionPackViewModel vm in e.NewItems)
                RegisterPackSubscriptions(vm);
        }

        if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null)
        {
            foreach (QuestionPackViewModel vm in e.OldItems)
                UnregisterPackSubscriptions(vm);
        }
        _ = ScheduleSaveAsync();

        ConfigurationViewModel?.NotifyPacksChanged();
    }

    private void RegisterPackSubscriptions(QuestionPackViewModel vm)
    {
        vm.PropertyChanged += PackVm_PropertyChanged;
        vm.Questions.CollectionChanged += PackQuestions_CollectionChanged;

        foreach (var q in vm.Questions)
        {
            q.PropertyChanged += Question_PropertyChanged;
            if (q.IncorrectAnswers is INotifyCollectionChanged inc)
                inc.CollectionChanged += QuestionIncorrectAnswers_CollectionChanged;
        }
    }

    private void UnregisterPackSubscriptions(QuestionPackViewModel vm)
    {
        vm.PropertyChanged -= PackVm_PropertyChanged;
        vm.Questions.CollectionChanged -= PackQuestions_CollectionChanged;

        foreach (var q in vm.Questions)
        {
            q.PropertyChanged -= Question_PropertyChanged;
            if (q.IncorrectAnswers is INotifyCollectionChanged inc)
                inc.CollectionChanged -= QuestionIncorrectAnswers_CollectionChanged;
        }
    }

    private void PackVm_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        _ = ScheduleSaveAsync();
    }

    private void PackQuestions_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
            foreach (Question q in e.NewItems)
            {
                q.PropertyChanged += Question_PropertyChanged;
                if (q.IncorrectAnswers is INotifyCollectionChanged inc)
                    inc.CollectionChanged += QuestionIncorrectAnswers_CollectionChanged;
            }

        if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null)
            foreach (Question q in e.OldItems)
            {
                q.PropertyChanged -= Question_PropertyChanged;
                if (q.IncorrectAnswers is INotifyCollectionChanged inc)
                    inc.CollectionChanged -= QuestionIncorrectAnswers_CollectionChanged;
            }

        if (e.Action == NotifyCollectionChangedAction.Replace && e.OldItems != null && e.NewItems != null)
        {
            var oldQ = (Question)e.OldItems[0]!;
            var newQ = (Question)e.NewItems[0]!;
            oldQ.PropertyChanged -= Question_PropertyChanged;
            if (oldQ.IncorrectAnswers is INotifyCollectionChanged oinc)
                oinc.CollectionChanged -= QuestionIncorrectAnswers_CollectionChanged;

            newQ.PropertyChanged += Question_PropertyChanged;
            if (newQ.IncorrectAnswers is INotifyCollectionChanged ninc)
                ninc.CollectionChanged += QuestionIncorrectAnswers_CollectionChanged;
        }

        if (e.Action == NotifyCollectionChangedAction.Reset)
        {
        }

        _ = ScheduleSaveAsync();
    }

    private void Question_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        _ = ScheduleSaveAsync();
    }

    private void QuestionIncorrectAnswers_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        _ = ScheduleSaveAsync();
    }

    private async Task ScheduleSaveAsync()
    {
        lock (_saveLock)
        {
            _saveCts?.Cancel();
            _saveCts = new CancellationTokenSource();
        }

        var cts = _saveCts!;
        try
        {
            await Task.Delay(400, cts.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            return;
        }

        try
        {
            await SavePacksAsync().ConfigureAwait(false);
        }
        catch
        {
        }
    }

    private async Task LoadPacksAsync()
    {
        var loaded = await QuestionPackStorage.LoadAsync().ConfigureAwait(false);

        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            Packs.Clear();
            foreach (var pack in loaded)
            {
                var vm = new QuestionPackViewModel(pack, this);
                Packs.Add(vm);
                RegisterPackSubscriptions(vm);
            }

            if (Packs.Count == 0)
            {
                var newPack = new QuestionPackViewModel(new QuestionPack("Default Question Pack"), this);
                Packs.Add(newPack);
                RegisterPackSubscriptions(newPack);
            }

            if (ActivePack == null && Packs.Count > 0)
                ActivePack = Packs.First();
        }).Task.ConfigureAwait(false);
    }

    public async Task SavePacksAsync()
    {
        var toSave = Packs.Select(p => p.Model).ToList();

        System.Diagnostics.Debug.WriteLine($"[SavePacksAsync] Saving {toSave.Count} packs to disk.");
        try
        {
            var preview = JsonSerializer.Serialize(toSave, new JsonSerializerOptions { WriteIndented = true });
            System.Diagnostics.Debug.WriteLine(preview);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("[SavePacksAsync] Failed to create JSON preview: " + ex);
        }

        await QuestionPackStorage.SaveAsync(toSave).ConfigureAwait(false);
    }
}
