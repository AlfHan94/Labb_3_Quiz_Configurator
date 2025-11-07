using Labb_3_Quiz_Configurator.Models;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Labb_3_Quiz_Configurator.Command;
using Labb_3_Quiz_Configurator.Dialogs;

namespace Labb_3_Quiz_Configurator.ViewModels
{
    class ConfigurationViewModel : ViewModelBase
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

        public ConfigurationViewModel(MainWindowViewModel? mainWindowViewModel)
        {
            this._mainWindowViewModel = mainWindowViewModel;
            AddQuestionCommand = new DelegateCommand(_ => AddQuestion());
            RemoveQuestionCommand = new DelegateCommand(_ => RemoveQuestion());
            OpenPackOptionsCommand = new DelegateCommand(_ => OpenPackOptions());
        }

        private void AddQuestion()
        {
            if (ActivePack != null)
            {
                var newQuestion = new Question(
                    "New Question",
                    "",
                    "",
                    "",
                    ""
                );

                ActivePack.Questions.Add(newQuestion);
                ActiveQuestion = newQuestion;
            }
        }

        private void RemoveQuestion()
        {
            if (ActivePack != null && ActiveQuestion != null)
            {
                ActivePack.Questions.Remove(ActiveQuestion);

                if (ActivePack.Questions.Count > 0)
                    ActiveQuestion = ActivePack.Questions[0];
                else
                    ActiveQuestion = null;
            }
        }

        private void OpenPackOptions()
        {
            var dialog = new PackOptionsDialog();
            dialog.DataContext = ActivePack;
            dialog.ShowDialog();
        }



    }
}
