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

        public DelegateCommand SetPackNameCommand { get; }
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
            SetPackNameCommand = new DelegateCommand(SetPackName, CanSetPackName);
            AnswerCommand = new DelegateCommand(Answer);
            StartQuiz();
            DemoText = string.Empty;

        }
        private string _demoText;
        public string DemoText
        {
            get { return _demoText; }
            set
            {
                _demoText = value;
                RaisePropertyChanged();
                SetPackNameCommand.RaiseCanExecuteChanged();
            }
        }

        private bool CanSetPackName(object? arg)
        {
            return DemoText.Length > 0;
        }

        private void SetPackName(object? obj)
        {
            ActivePack.Name= DemoText;
            RaisePropertyChanged();
        }


    }
}
