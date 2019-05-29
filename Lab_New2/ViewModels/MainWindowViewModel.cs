using Lab_New2.Data;
using Lab_New2.ViewModels;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Lab_New2
{
    public class MainWindowViewModel : BindableBase
    {
        private ObservableCollection<Agent> agents;
        private string filePath = "";
        private DispatcherTimer timer = new DispatcherTimer();
        private string filter;
        private readonly string AppTitle = "Agent Assignments 5.4";

        public MainWindowViewModel()
        {
            agents = new ObservableCollection<Agent>
            {
                #if DEBUG
                new Agent("001", "Nina", "Assasination", "UpperVolta"),
                new Agent("007", "James Bond", "Martinis", "North Korea")
                #endif
            };
            CurrentAgent = null;

            Specialities = new ObservableCollection<string>
            {
                "Assasination",
                "Bombs",
                "Martinis",
                "Low profile",
                "Spy"
            };

            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Start();
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            clock.Update();
        }

        #region Properties

        public ObservableCollection<Agent> Agents
        {
            get { return agents; }
            set { SetProperty(ref agents, value); }
        }

        Agent currentAgent = null;

        public Agent CurrentAgent
        {
            get { return currentAgent; }
            set
            {
                SetProperty(ref currentAgent, value);
            }
        }

        int currentIndex = -1;
        public int CurrentIndex
        {
            get { return currentIndex; }
            set
            {
                SetProperty(ref currentIndex, value);
            }
        }

        Clock clock = new Clock();
        public Clock Clock { get => clock; set => clock = value; }

        ObservableCollection<string> specialities;
        public ObservableCollection<string> Specialities
        {
            get { return specialities; }
            set
            {
                SetProperty(ref specialities, value);
            }
        }

        Object _selectedSortOrder = "None";
        public Object SelectedSortOrder
        {
            get { return _selectedSortOrder; }
            set
            {
                SetProperty(ref _selectedSortOrder, value);
                ICollectionView cv = CollectionViewSource.GetDefaultView(Agents);
                var newSortOrder = value as ComboBoxItem;
                var sortDesc = new SortDescription(newSortOrder.Tag.ToString(), ListSortDirection.Ascending);
                if (cv != null)
                {
                    cv.SortDescriptions.Clear();
                    if (newSortOrder.Tag.ToString() != "None")
                        cv.SortDescriptions.Add(sortDesc);
                }
            }
        }

        public IReadOnlyCollection<string> FilterSpecialities
        {
            get
            {
                ObservableCollection<string> result = new ObservableCollection<string>
                {
                    "All"
                };
                result.AddRange(Specialities);
                return result;
            }
        }

        int currentSpecialityIndex = 0;

        public int CurrentSpecialityIndex
        {
            get { return currentSpecialityIndex; }
            set
            {
                if (currentSpecialityIndex != value)
                {
                    ICollectionView cv = CollectionViewSource.GetDefaultView(Agents);
                    currentSpecialityIndex = value;
                    if (currentSpecialityIndex == 0)
                        cv.Filter = null; // Index 0 is no filter (show all)
                    else
                    {
                        filter = Specialities[(currentSpecialityIndex - 1)];
                        cv.Filter = CollectionViewSource_Filter;
                    }
                    RaisePropertyChanged();
                }
            }
        }

        private bool CollectionViewSource_Filter(object ag)
        {
            Agent agent = ag as Agent;
            return (agent.Speciality == filter);
        }

        private string filename = "";
        public string Filename
        {
            get { return filename; }
            set
            {
                SetProperty(ref filename, value);
                RaisePropertyChanged("Title");
            }
        }

        public string Title
        {
            get
            {
                var s = "";
                if (Dirty)
                    s = "*";
                return Filename + s + " - " + AppTitle;
            }
        }

        private bool dirty = false;
        public bool Dirty
        {
            get { return dirty; }
            set
            {
                SetProperty(ref dirty, value);
                RaisePropertyChanged("Title");
            }
        }

        #endregion

        #region Commands

        ICommand _PreviusCommand;
        public ICommand PreviusCommand
        {
            get
            {
                return _PreviusCommand ??
                (_PreviusCommand = new DelegateCommand(
                 PreviusCommandExecute, PreviusCommandCanExecute).ObservesProperty(() => CurrentIndex));
            }
        }

        private void PreviusCommandExecute()
        {
            if (CurrentIndex > 0)
                --CurrentIndex;
        }

        private bool PreviusCommandCanExecute()
        {
            if (CurrentIndex > 0)
                return true;
            else
                return false;
        }

        ICommand _nextCommand;
        public ICommand NextCommand
        {
            get
            {
                return _nextCommand ?? (_nextCommand = new DelegateCommand(
                       () => ++CurrentIndex,
                       () => CurrentIndex < (Agents.Count - 1)).ObservesProperty(() => CurrentIndex));
            }
        }

        ICommand _newCommand;
        public ICommand AddNewAgentCommand
        {
            get
            {
                return _newCommand ?? (_newCommand = new DelegateCommand(() =>
                {
                    var newAgent = new Agent();
                    var vm = new AgentViewModel("Add new agent", newAgent)
                    {
                        Specialities = specialities
                    };
                    var dlg = new AgentWiew
                    {
                        DataContext = vm
                    };
                    if (dlg.ShowDialog() == true)
                    {
                        Agents.Add(newAgent);
                        CurrentAgent = newAgent;
                        CurrentSpecialityIndex = 0;
                        Dirty = true;
                    }
                }));
            }
        }

        ICommand _deleteCommand;
        public ICommand DeleteAgentCommand => _deleteCommand ?? (_deleteCommand =
            new DelegateCommand(DeleteAgent, DeleteAgent_CanExecute)
                    .ObservesProperty(() => CurrentIndex));

        private void DeleteAgent()
        {
            MessageBoxResult res = MessageBox.Show("Are you sure you want to delete agent " + CurrentAgent.CodeName +
                "?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (res == MessageBoxResult.Yes)
            {
                Agents.Remove(CurrentAgent);
                Dirty = true;
            }
        }

        private bool DeleteAgent_CanExecute()
        {
            if (Agents.Count > 0 && CurrentIndex >= 0)
                return true;
            else
                return false;
        }

        ICommand _editCommand;
        public ICommand EditAgentCommand
        {
            get
            {
                return _editCommand ?? (_editCommand = new DelegateCommand(() =>
                {
                    var tempAgent = CurrentAgent.Clone();
                    var vm = new AgentViewModel("Edit agent", tempAgent)
                    {
                        Specialities = specialities
                    };
                    var dlg = new AgentWiew
                    {
                        DataContext = vm,
                        Owner = App.Current.MainWindow
                    };
                    if (dlg.ShowDialog() == true)
                    {
                        // Copy values back
                        CurrentAgent.ID = tempAgent.ID;
                        CurrentAgent.CodeName = tempAgent.CodeName;
                        CurrentAgent.Speciality = tempAgent.Speciality;
                        CurrentAgent.Assignment = tempAgent.Assignment;
                        Dirty = true;
                    }
                },
                () => {
                    return CurrentIndex >= 0;
                }
                ).ObservesProperty(() => CurrentIndex));
            }
        }

        ICommand _closeAppCommand;
        public ICommand CloseAppCommand
        {
            get
            {
                return _closeAppCommand ?? (_closeAppCommand = new DelegateCommand(() =>
                {
                    Application.Current.MainWindow.Close();
                }));
            }
        }

        ICommand _SaveAsCommand;
        public ICommand SaveAsCommand
        {
            get { return _SaveAsCommand ?? (_SaveAsCommand = new DelegateCommand<string>(SaveAsCommand_Execute)); }
        }

        private void SaveAsCommand_Execute(string argFilename)
        {
            var dialog = new SaveFileDialog
            {
                Filter = "Agent assignment documents|*.agn|All Files|*.*",
                DefaultExt = "agn"
            };
            if (filePath == "")
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            else
                dialog.InitialDirectory = Path.GetDirectoryName(filePath);

            if (dialog.ShowDialog(App.Current.MainWindow) == true)
            {
                filePath = dialog.FileName;
                Filename = Path.GetFileName(filePath);
                SaveFile();
            }
        }

        ICommand _SaveCommand;
        public ICommand SaveCommand
        {
            get
            {
                return _SaveCommand ?? (_SaveCommand = new DelegateCommand(SaveFileCommand_Execute, SaveFileCommand_CanExecute)
                  .ObservesProperty(() => Agents.Count));
            }
        }

        private void SaveFileCommand_Execute()
        {
            SaveFile();
        }

        private bool SaveFileCommand_CanExecute()
        {
            return (filename != "") && (Agents.Count > 0);
        }

        private void SaveFile()
        {
            try
            {
                Repository.SaveFile(filePath, Agents);
                Dirty = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Unable to save file", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        ICommand _NewFileCommand;
        public ICommand NewFileCommand
        {
            get { return _NewFileCommand ?? (_NewFileCommand = new DelegateCommand(NewFileCommand_Execute)); }
        }

        private void NewFileCommand_Execute()
        {
            MessageBoxResult res = MessageBox.Show("Any unsaved data will be lost. Are you sure you want to initiate a new file?", "Warning",
                MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (res == MessageBoxResult.Yes)
            {
                Agents.Clear();
                Filename = "";
                Dirty = false;
            }
        }


        ICommand _OpenFileCommand;
        public ICommand OpenFileCommand
        {
            get { return _OpenFileCommand ?? (_OpenFileCommand = new DelegateCommand<string>(OpenFileCommand_Execute)); }
        }

        private void OpenFileCommand_Execute(string argFilename)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Agent assignment documents|*.agn|All Files|*.*",
                DefaultExt = "agn"
            };
            if (filePath == "")
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            else
                dialog.InitialDirectory = Path.GetDirectoryName(filePath);

            if (dialog.ShowDialog(App.Current.MainWindow) == true)
            {
                filePath = dialog.FileName;
                Filename = Path.GetFileName(filePath);
                try
                {
                    Repository.ReadFile(filePath, out ObservableCollection<Agent> tempAgents);
                    Agents = tempAgents;
                    Dirty = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Unable to open file", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        ICommand _closingCommand;
        public ICommand ClosingCommand
        {
            get
            {
                return _closingCommand ?? (_closingCommand = new
                  DelegateCommand<CancelEventArgs>(ClosingCommand_Execute));
            }
        }

        private void ClosingCommand_Execute(CancelEventArgs arg)
        {
            if (Dirty)
                arg.Cancel = UserRegrets();
        }

        private bool UserRegrets()
        {
            var regret = false;
            MessageBoxResult res = MessageBox.Show("You have unsaved data. Are you sure you want to close the application without saving data first?",
                            "Warning", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (res == MessageBoxResult.No)
            {
                regret = true;
            }
            return regret;
        }

        ICommand _ColorCommand;
        public ICommand ColorCommand
        {
            get
            {
                return _ColorCommand ?? (_ColorCommand = new
                  DelegateCommand<String>(ColorCommand_Execute));
            }
        }

        private void ColorCommand_Execute(String colorStr)
        {
            SolidColorBrush newBrush = SystemColors.WindowBrush; // Default color

            try
            {
                if (colorStr != null)
                {
                    if (colorStr != "Default")
                        newBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(colorStr));
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Unknown color name, default color is used", "Program error!");
            }

            Application.Current.Resources["BackgroundBrush"] = newBrush;
        }

        #endregion
    }
}
