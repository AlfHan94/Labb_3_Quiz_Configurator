using Labb_3_Quiz_Configurator.Command;
using Labb_3_Quiz_Configurator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb_3_Quiz_Configurator.ViewModels
{
    class PlayerViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel? _mainWindowViewModel;
        public DelegateCommand AnswerCommand { get; }


        public QuestionPackViewModel? ActivePack { get => _mainWindowViewModel?.ActivePack; }

        private int _currentQuestionIndex;
        public Question CurrentQuestion => ActivePack?.Questions[_currentQuestionIndex]?.Model;

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

        public int Score { get; private set; }
        public bool IsAnswering { get; private set; } = true;


        public PlayerViewModel(MainWindowViewModel? mainWindowViewModel)
        {
            this._mainWindowViewModel = mainWindowViewModel;
            AnswerCommand = new DelegateCommand(Answer);
            StartQuiz();
        }


        private void StartQuiz()
        {
            _currentQuestionIndex = 0;
            Score = 0;
            LoadQuestion();
        }
        
        private void LoadQuestion()
        {
            SelectedAnswer = null;
            IsAnswering = true;

            var answers = new List<string> { CurrentQuestion.CorrectAnswer };
            answers.AddRange(CurrentQuestion.IncorrectAnswers);
            ShuffledAnswers = answers.OrderBy(x => Guid.NewGuid()).ToList();

            RaisePropertyChanged(nameof(CurrentQuestion));
        }

        private async void Answer(object selected)
        {
            if (!IsAnswering)
                return;

            IsAnswering = false;
            SelectedAnswer = selected as string;

            if (SelectedAnswer == CurrentQuestion.CorrectAnswer)
                Score++;

            RaisePropertyChanged(nameof(Score));
            RaisePropertyChanged(nameof(SelectedAnswer));

            await Task.Delay(2000);

            _currentQuestionIndex++;
            
            if (_currentQuestionIndex >= ActivePack.Questions.Count)
            {
                ShowResultScreen();
                return;
            }
        }

        private void ShowResultScreen()
        {
            _mainWindowViewModel.CurrentView = new ResultView(_mainWindowViewModel, Score, ActivePack.Questions.Count);
        }


    }
}
