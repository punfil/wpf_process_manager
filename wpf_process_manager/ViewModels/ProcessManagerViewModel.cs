using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wpf_process_manager.Models;
using wpf_process_manager.ProcessManager;

namespace wpf_process_manager.ViewModels
{
    public class ProcessManagerViewModel : NotifyPropertyChanged
    {
        private ProcessManager.ProcessManager _processManager;
        public ObservableCollection<ProcessPriorityModel> ProcessPriorities { get; set; }
        private int _intervalValue;

        public int IntervalValue
        {
            get { return _intervalValue; }
            set
            {
                _intervalValue = value;
                OnPropertyChanged();
            }
        }

        public Command<object> AutoRefreshCommand { get; private set; }
        public Command<object> KillCommand { get; private set; }
        public Command<object> SetPriorityCommand { get; private set; }


        public ProcessManagerViewModel()
        {
            _processManager = new ProcessManager.ProcessManager();
            ProcessPriorities = new ObservableCollection<ProcessPriorityModel>();
            foreach (var priority in Enum.GetValues(typeof(ProcessPriorityClass)))
            {
                ProcessPriorities.Add(new ProcessPriorityModel((ProcessPriorityClass)priority));
            }

            AutoRefreshCommand = new Command<object>(
                _ => AutoRefresh(),
                _ => ReturnTrue()
            );
            KillCommand = new Command<object>(
                _ => Kill(),
                _ => ReturnTrue()
            );

        }

        private bool ReturnTrue()
        {
            return true;
        }

        /* Commands */
        private void Kill()
        {
            //_processManager.KillProcess()
        }
        private void AutoRefresh()
        {
            _processManager.AutoRefresh();
        }
    }
}
