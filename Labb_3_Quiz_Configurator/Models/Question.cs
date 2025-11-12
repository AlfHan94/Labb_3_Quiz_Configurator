using System.Collections.ObjectModel;
using Labb_3_Quiz_Configurator.ViewModels;

namespace Labb_3_Quiz_Configurator.Models;

public class Question : ViewModelBase
{
    private string _query = "";
    private string _correctAnswer = "";
    private ObservableCollection<string> _incorrectAnswers = new();

    public Question() { }

    public Question(string query, string correctAnswer,
        string incorrectAnswer1, string incorrectAnswer2, string incorrectAnswer3)
    {
        _query = query ?? "";
        _correctAnswer = correctAnswer ?? "";
        _incorrectAnswers = new ObservableCollection<string>()
        {
            incorrectAnswer1 ?? "",
            incorrectAnswer2 ?? "",
            incorrectAnswer3 ?? ""
        };
    }

    public string Query
    {
        get => _query;
        set
        {
            if (_query == value) return;
            _query = value ?? "";
            RaisePropertyChanged();
        }
    }

    public string CorrectAnswer
    {
        get => _correctAnswer;
        set
        {
            if (_correctAnswer == value) return;
            _correctAnswer = value ?? "";
            RaisePropertyChanged();
        }
    }

    public ObservableCollection<string> IncorrectAnswers
    {
        get => _incorrectAnswers;
        set
        {
            if (ReferenceEquals(_incorrectAnswers, value)) return;
            _incorrectAnswers = value ?? new ObservableCollection<string>();
            RaisePropertyChanged();
        }
    }
}
