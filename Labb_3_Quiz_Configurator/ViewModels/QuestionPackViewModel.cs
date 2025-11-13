using Labb_3_Quiz_Configurator.Models;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
namespace Labb_3_Quiz_Configurator.ViewModels;

public class QuestionPackViewModel : ViewModelBase
{
    private readonly MainWindowViewModel? _mainWindowViewModel;

    private readonly QuestionPack _model;
    public QuestionPack Model => _model;
    public ObservableCollection<Question> Questions { get; set; }

    
    public QuestionPackViewModel(QuestionPack model, MainWindowViewModel? mainWindowViewModel = null)
    {
        _model = model;
        _mainWindowViewModel = mainWindowViewModel;

        Questions = new ObservableCollection<Question>(_model.Questions);
        Questions.CollectionChanged += Questions_CollectionChanged;

        SubscribeToQuestionChanges();
    }

    private void SubscribeToQuestionChanges()
    {
        foreach (var q in Questions)
            q.PropertyChanged += (_, __) =>
            {

            };
    }

    private void Questions_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
            foreach (Question q in e.NewItems) _model.Questions.Add(q);

        if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null)
            foreach (Question q in e.OldItems) _model.Questions.Remove(q);

        if (e.Action == NotifyCollectionChangedAction.Replace && e.OldItems != null && e.NewItems != null)
            _model.Questions[e.OldStartingIndex] = (Question)e.NewItems[0]!;
        if (e.Action == NotifyCollectionChangedAction.Reset)
            _model.Questions.Clear();

    }

    public string Name
    {
        get => _model.Name;
        set
        {
            _model.Name = value;
            RaisePropertyChanged();
        }
    }
    public Difficulty Difficulty
    {
        get => _model.Difficulty;
        set
        {
            _model.Difficulty = value;
            RaisePropertyChanged();
        }
    }
    public int TimeLimitInSeconds
    {
        get => _model.TimeLimitInSeconds;
        set
        {
            _model.TimeLimitInSeconds = value;
            RaisePropertyChanged();
        }
    }
    



}
