using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using wpf_process_manager.Models;
using wpf_process_manager.ProcessManager;

namespace wpf_process_manager.ViewModels
{
    public class ProcessManagerViewModel : NotifyPropertyChanged
    {
        private readonly ProcessManager.ProcessManager _processManager;
        public ObservableCollection<ProcessPriorityModel> ProcessPriorities { get; set; }

        private ObservableCollection<ProcessModel> _processes;
        public ObservableCollection<ProcessModel> Processes
        {
            get => _processes;
            set
            {
                _processes = value;
                OnPropertyChanged();
            }
        }

        private int _intervalValue;

        public int IntervalValue
        {
            get => _intervalValue;
            set
            {
                _intervalValue = value;
                OnPropertyChanged();
            }
        }

        private string _nameFilter;

        public string NameFilter
        {
            get => _nameFilter;
            set
            {
                _nameFilter = value;
                OnPropertyChanged();
            }
        }

        private string _cpuUsageFilter;

        public string CpuUsageFilter
        {
            get => _cpuUsageFilter;
            set
            {
                _cpuUsageFilter = value;
                OnPropertyChanged();
            }
        }

        private string _memoryUsageFilter;

        public string MemoryUsageFilter
        {
            get => _memoryUsageFilter;
            set
            {
                _memoryUsageFilter = value;
                OnPropertyChanged();
            }
        }

        private string _priorityFilter;

        public string PriorityFilter
        {
            get => _priorityFilter;
            set
            {
                _priorityFilter = value;
                OnPropertyChanged();
            }
        }

        private Dictionary<string, string> _selectedProcessDetails;

        public Dictionary<string, string> SelectedProcessDetails
        {
            get => _selectedProcessDetails;
            set
            {
                _selectedProcessDetails = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<int> _selectedProcessDetailsThreads;

        public ObservableCollection<int> SelectedProcessDetailsThreads
        {
            get => _selectedProcessDetailsThreads;
            set
            {
                _selectedProcessDetailsThreads = value;
                OnPropertyChanged();
            }
        }

        private ProcessPriorityModel _selectedProcessPriority;

        public ProcessPriorityModel SelectedProcessPriority
        {
            get => _selectedProcessPriority;
            set
            {
                _selectedProcessPriority = value;
                OnPropertyChanged();
            }
        }

        private ProcessModel _selectedProcess;

        public ProcessModel SelectedProcess
        {
            get => _selectedProcess;
            set
            {
                _selectedProcess = value;
                ShowDetails();
                OnPropertyChanged();
            }
        }

        public Command<object> RefreshCommand { get; private set; }
        public Command<object> AutoRefreshCommand { get; private set; }
        public Command<object> KillCommand { get; private set; }
        public Command<object> SetPriorityCommand { get; private set; }
        public Command<object> FilterProcessesCommand { get; private set; }

        public ProcessManagerViewModel()
        {
            _processManager = new ProcessManager.ProcessManager();
            _processManager.RefreshTimerHandler += (sender, args) =>
            {
                if (args is AutoRefreshReturn refreshReturn)
                {
                    Processes = new ObservableCollection<ProcessModel>(refreshReturn.processesList);
                    ClearFilters();
                    SelectedProcessDetails = null;
                    SelectedProcessDetailsThreads = null;
                }
            };
            ProcessPriorities = new ObservableCollection<ProcessPriorityModel>();
            Processes = new ObservableCollection<ProcessModel>(_processManager.Refresh());
            foreach (var priority in Enum.GetValues(typeof(ProcessPriorityClass)))
            {
                ProcessPriorities.Add(new ProcessPriorityModel((ProcessPriorityClass)priority));
            }

            // Show sth by default
            SelectedProcessPriority = ProcessPriorities[0];

            AutoRefreshCommand = new Command<object>(
                _ => AutoRefresh(),
                _ => ReturnTrue()
            );
            KillCommand = new Command<object>(
                _ => Kill(),
                _ => ReturnTrue()
            );
            SetPriorityCommand = new Command<object>(
                _ => SetPriority(),
                _ => ReturnTrue()
            );
            RefreshCommand = new Command<object>(
                _ => Refresh(),
                _ => ReturnTrue()
            );
            FilterProcessesCommand = new Command<object>(
                _ => FilterProcesses(),
                _ => ReturnTrue()
            );
        }

        private bool ReturnTrue()
        {
            return true;
        }

        /* Commands */
        private void Refresh()
        {
            var thread = new Thread(RefreshThreadWrapper);
            thread.Start();
        }

        private void RefreshThreadWrapper()
        {
            Processes = new ObservableCollection<ProcessModel>(_processManager.Refresh());
            ClearFilters();
            SelectedProcessDetails = null;
            SelectedProcessDetailsThreads = null;
        }

        private void FilterProcesses()
        {
            Processes = new ObservableCollection<ProcessModel>(
                _processManager.FilterProcesses(NameFilter, CpuUsageFilter, MemoryUsageFilter, PriorityFilter));
        }

        private void ClearFilters()
        {
            NameFilter = "";
            CpuUsageFilter = "";
            MemoryUsageFilter = "";
            PriorityFilter = "";
        }

        private void ShowDetails()
        {
            if (SelectedProcess != null)
            {
                SelectedProcessDetails = _processManager.GetProcessDetails(SelectedProcess);
                SelectedProcessDetailsThreads = new ObservableCollection<int>(_processManager.GetProcessThreadsDetails(SelectedProcess));
            }
        }

        private void Kill()
        {
            var retval = _processManager.KillProcess(SelectedProcess);
            if (retval)
            {
                Refresh();
                SelectedProcess = null;
            }
            else
            {
                MessageBox.Show($"Can't kill selected process", "Error",MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void AutoRefresh()
        {
            _processManager.AutoRefresh(IntervalValue);
        }

        private void SetPriority()
        {
            var retval = _processManager.SetProcessPriority(SelectedProcess, SelectedProcessPriority);
            if (retval)
            {
                Refresh();
                SelectedProcess = null;
            }
            else
            {
                MessageBox.Show($"Can't set priority of selected process", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
