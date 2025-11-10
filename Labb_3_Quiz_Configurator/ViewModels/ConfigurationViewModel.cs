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
        public ICommand OpenNewQuestionPackCommand { get; }


        public ConfigurationViewModel(MainWindowViewModel? mainWindowViewModel)
        {
            this._mainWindowViewModel = mainWindowViewModel;
            _mainWindowViewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(MainWindowViewModel.ActivePack))
                {
                    RaisePropertyChanged(nameof(ActivePack));
                    ActiveQuestion = ActivePack?.Questions.FirstOrDefault();
                }
            };

            AddQuestionCommand = new DelegateCommand(_ => AddQuestion());
            RemoveQuestionCommand = new DelegateCommand(_ => RemoveQuestion());
            OpenPackOptionsCommand = new DelegateCommand(_ => OpenPackOptions());
            OpenNewQuestionPackCommand = new DelegateCommand(_ => OpenNewQuestionPack());
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
            ActiveQuestion = ActivePack.Questions.FirstOrDefault();
        }

        private void OpenPackOptions()
        {
            var dialog = new PackOptionsDialog();
            dialog.DataContext = ActivePack;
            dialog.ShowDialog();
        }

        private void OpenNewQuestionPack()
        {
            var newPack = new QuestionPackViewModel(new QuestionPack("New Pack"));

            var dialog = new CreateNewPackDialog();
            dialog.DataContext = newPack;

            if (dialog.ShowDialog() == true)
            {
                _mainWindowViewModel.Packs.Add(newPack);
                _mainWindowViewModel.ActivePack = newPack;
            }
        }


    }
}
