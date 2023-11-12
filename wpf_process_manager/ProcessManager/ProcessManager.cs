using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using wpf_process_manager.Models;

namespace wpf_process_manager.ProcessManager
{
    public class ProcessManager
    {
        private List<ProcessModel> _processes;
        private DispatcherTimer _refreshTimer;
        public ProcessManager()
        {
            _processes = new List<ProcessModel>();
            _refreshTimer = new DispatcherTimer();
            _refreshTimer.Interval = TimeSpan.FromSeconds(3); // TODO
            _refreshTimer.Tick += AutoRefresh;
        }

        public Process[] GetRunningProcesses()
        {
            return Process.GetProcesses();

        }

        private void OnRequestTickData()
        {
            // TODO: return values
        }

        private void AutoRefresh(object sender, EventArgs e)
        {
            Refresh();
        }

        public List<ProcessModel> Refresh()
        {
            var finishedProcesses = _processes.RemoveAll(x => x.HasFinished() == true);
            Process[] processes = GetRunningProcesses();

            // Update processes that already exist in the list
            foreach (Process process in processes)
            {
                var proc = _processes.Find(x => x.GetPID() == process.Id);
                if (proc == null)
                {
                    _processes.Add(new ProcessModel(process));
                }
                else
                {
                    proc.Refresh();
                }
            }

            if (_refreshTimer.IsEnabled)
            {
                OnRequestTickData();
            }

            return this._processes;
        }
    }
}
