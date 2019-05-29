using Prism.Commands;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Vittigheder1.ViewModels
{
    public class AgentViewModel : BindableBase
    {
        public AgentViewModel(string title, Agent agent)
        {
            Title = title;
            CurrentAgent = agent;
        }

        #region Properties
        string title;

        public string Title
        {
            get { return title; }
            set
            {
                SetProperty(ref title, value);
            }
        }
        Agent currentAgent;

        public Agent CurrentAgent
        {
            get { return currentAgent; }
            set
            {
                SetProperty(ref currentAgent, value);
            }
        }

        //bool isValid;

        public bool IsValid
        {
            get
            {
                bool isValid = true;
                if (string.IsNullOrWhiteSpace(CurrentAgent.ID))
                    isValid = false;
                if (string.IsNullOrWhiteSpace(CurrentAgent.CodeName))
                    isValid = false;
                return isValid;
            }
            //set
            //{
            //    SetProperty(ref isValid, value);
            //}
        }

        ObservableCollection<string> specialities;
        public ObservableCollection<string> Specialities
        {
            get { return specialities; }
            set
            {
                SetProperty(ref specialities, value);
            }
        }
        #endregion

        #region Commands

        ICommand _okBtnCommand;
        public ICommand OkBtnCommand
        {
            get
            {
                return _okBtnCommand ?? (_okBtnCommand = new DelegateCommand(
                    OkBtnCommand_Execute, OkBtnCommand_CanExecute)
                  .ObservesProperty(() => CurrentAgent.ID)
                  .ObservesProperty(() => CurrentAgent.CodeName));
            }
        }

        private void OkBtnCommand_Execute()
        {
            // Nothing needs to be done here
        }

        private bool OkBtnCommand_CanExecute()
        {
            return IsValid;
        }

        #endregion
    }
}
