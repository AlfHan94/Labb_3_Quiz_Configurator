using Labb_3_Quiz_Configurator.Command;
using Labb_3_Quiz_Configurator.Dialogs;
using Labb_3_Quiz_Configurator.Models;
using System.Collections.Specialized;
using System.Windows.Input;
namespace Labb_3_Quiz_Configurator.ViewModels;

public class ConfigurationViewModel : ViewModelBase
{
    private readonly MainWindowViewModel? _mainWindowViewModel;
    private Question _activeQuestion;

    public QuestionPackViewModel? ActivePack { get => _mainWindowViewModel?.ActivePack; }
    public Question ActiveQuestion
    {
        get => _activeQuestion;
        set
        {
            _activeQuestion = value;
            RaisePropertyChanged();
        }
    }
    public ICommand AddQuestionCommand { get; }
    public ICommand RemoveQuestionCommand { get; }
    public ICommand OpenPackOptionsCommand { get; }
    public ICommand OpenNewQuestionPackCommand { get; }
    public ICommand DeleteQuestionPackCommand { get; }

    public ConfigurationViewModel(MainWindowViewModel? mainWindowViewModel)
    {
        this._mainWindowViewModel = mainWindowViewModel;
        _mainWindowViewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(MainWindowViewModel.ActivePack))
            {
                RaisePropertyChanged(nameof(ActivePack));
                ActiveQuestion = ActivePack?.Questions?.FirstOrDefault();
            }
        };

        AddQuestionCommand = new DelegateCommand(_ => AddQuestion());
        RemoveQuestionCommand = new DelegateCommand(_ => RemoveQuestion());
        OpenPackOptionsCommand = new DelegateCommand(_ => OpenPackOptions());
        OpenNewQuestionPackCommand = new DelegateCommand(_ => OpenNewQuestionPack());
        DeleteQuestionPackCommand = new DelegateCommand(_ => DeleteQuestionPack(), _ => CanDeletePack());

        if (_mainWindowViewModel?.Packs is INotifyCollectionChanged packs)
        {
            packs.CollectionChanged += (_, __) =>
                ((DelegateCommand)DeleteQuestionPackCommand).RaiseCanExecuteChanged();
        }
    }

    private void AddQuestion()
    {
        if (ActivePack != null)
        {
            var newQuestion = new Question("New Question", "", "", "", "");
            ActivePack.Questions.Add(newQuestion);
            ActiveQuestion = newQuestion;

            _ = _mainWindowViewModel?.SavePacksAsync();
        }
    }
    private void RemoveQuestion()
    {
        if (ActivePack != null && ActiveQuestion != null)
        {
            ActivePack.Questions.Remove(ActiveQuestion);
            ActiveQuestion = ActivePack.Questions.FirstOrDefault();

            _ = _mainWindowViewModel?.SavePacksAsync();
        }
    }

    private void OpenPackOptions()
    {
        var dialog = new PackOptionsDialog { DataContext = ActivePack };
        dialog.ShowDialog();

        _ = _mainWindowViewModel?.SavePacksAsync();
    }

    private void OpenNewQuestionPack()
    {
        var dialog = new CreateNewPackDialog();
        var vm = dialog.ViewModel;

        if (dialog.ShowDialog() == true)
        {
            var pack = new QuestionPack(vm.Name, vm.Difficulty, vm.TimeLimitInSeconds);
            var newPackVm = new QuestionPackViewModel(pack);

            _mainWindowViewModel.Packs.Add(newPackVm);
            _mainWindowViewModel.ActivePack = newPackVm;

            _ = _mainWindowViewModel.SavePacksAsync();
        }
    }

    private void DeleteQuestionPack()
    {
        if (_mainWindowViewModel == null || ActivePack == null)
            return;
        if (_mainWindowViewModel.Packs.Count <= 1)
            return;

        _mainWindowViewModel.Packs.Remove(ActivePack);
        _mainWindowViewModel.ActivePack = _mainWindowViewModel.Packs.FirstOrDefault();

        _ = _mainWindowViewModel.SavePacksAsync();
    }
    private bool CanDeletePack()
    {
        return _mainWindowViewModel?.Packs?.Count > 1;
    }
    public void NotifyPacksChanged() =>
    ((DelegateCommand)DeleteQuestionPackCommand).RaiseCanExecuteChanged();


}
