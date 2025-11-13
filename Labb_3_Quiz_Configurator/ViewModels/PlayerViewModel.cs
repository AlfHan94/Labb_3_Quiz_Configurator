using Labb_3_Quiz_Configurator.Command;
using Labb_3_Quiz_Configurator.Models;
namespace Labb_3_Quiz_Configurator.ViewModels;

public class PlayerViewModel : ViewModelBase
{
    private readonly MainWindowViewModel? _mainWindowViewModel;
    public DelegateCommand AnswerCommand { get; }
    public QuestionPackViewModel? ActivePack => _mainWindowViewModel?.ActivePack;

    private int _currentQuestionIndex;
    public int CurrentQuestionIndex => _currentQuestionIndex + 1;

    public Question? CurrentQuestion
        => (ActivePack != null && _currentQuestionIndex < ActivePack.Questions.Count)
        ? ActivePack.Questions[_currentQuestionIndex] : null;

    private List<string> _shuffledAnswers;
    public List<string> ShuffledAnswers
    {
        get => _shuffledAnswers;
        set { _shuffledAnswers = value; RaisePropertyChanged(); }
    }

    private string _selectedAnswer;
    public string SelectedAnswer
    {
        get => _selectedAnswer;
        set { _selectedAnswer = value; RaisePropertyChanged(); }
    }

    public string CorrectAnswer => CurrentQuestion?.CorrectAnswer;
    public bool ShowFeedback => !IsAnswering;

    public int Score { get; private set; }

  
    private bool _isAnswering = true;
    public bool IsAnswering
    {
        get => _isAnswering;
        private set
        {
            _isAnswering = value;
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(ShowFeedback));
        }
    }

    private int _timeRemaining;
    public int TimeRemaining
    {
        get => _timeRemaining;
        set { _timeRemaining = value; RaisePropertyChanged(); }
    }


    public PlayerViewModel(MainWindowViewModel? mainWindowViewModel)
    {
        _mainWindowViewModel = mainWindowViewModel;
        AnswerCommand = new DelegateCommand(Answer);
    }

    public void StartQuiz()
    {
        if (ActivePack == null || ActivePack.Questions.Count == 0)
            return;

        _currentQuestionIndex = 0;
        Score = 0;
        LoadQuestion();
    }

    private void LoadQuestion()
    {
        SelectedAnswer = null;
        IsAnswering = true;

        var q = CurrentQuestion;
        if (q == null) return;

        var answers = new List<string> { q.CorrectAnswer };
        answers.AddRange(q.IncorrectAnswers);
        ShuffledAnswers = answers.OrderBy(_ => Guid.NewGuid()).ToList();

        TimeRemaining = ActivePack.TimeLimitInSeconds;
        StartTimer();

        RaisePropertyChanged(nameof(CurrentQuestion));
        RaisePropertyChanged(nameof(CurrentQuestionIndex));
    }

    private async void Answer(object selected)
    {
        if (!IsAnswering) return;

        IsAnswering = false;
        SelectedAnswer = selected as string;

        if (SelectedAnswer == CurrentQuestion.CorrectAnswer)
            Score++;

        RaisePropertyChanged(nameof(Score));
        RaisePropertyChanged(nameof(SelectedAnswer));
        RaisePropertyChanged(nameof(CorrectAnswer));

        await Task.Delay(2000);

        _currentQuestionIndex++;

        if (_currentQuestionIndex >= ActivePack.Questions.Count)
        {
            ShowResultScreen();
            return;
        }

        LoadQuestion();
    }

    private void ShowResultScreen()
    {
        _mainWindowViewModel.CurrentView = new ResultViewModel(_mainWindowViewModel, Score, ActivePack.Questions.Count);
    }

    private async void StartTimer()
    {
        while (TimeRemaining > 0 && IsAnswering)
        {
            await Task.Delay(1000);
            TimeRemaining--;
        }
        if (IsAnswering)
        {
            Answer(null);
        }
           
    }
}
