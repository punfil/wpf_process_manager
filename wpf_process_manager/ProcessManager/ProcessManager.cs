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

        private Process[] GetRunningProcesses()
        {
            return Process.GetProcesses();

        }

        public void OnRequestTickData()
        {
            // TODO: return values
        }

        private void AutoRefresh(object sender, EventArgs e)
        {
            Refresh();
        }

        public bool KillProcess(int pid)
        {
            var process = this._processes.Find(x => x.PID == pid);
            if (process != null && process.HasFinished())
            {//TODO: Remove from list?
                return process.Kill();
            }
            return false;
        }

        public void AutoRefresh()
        {
            if (!_refreshTimer.IsEnabled)
            {
                _refreshTimer.Start();
            }
            else
            {
                _refreshTimer.Stop();
            }
        }

        public List<ProcessModel> Refresh()
        {
            var currentIds = new HashSet<int>(_processes.Select(p => p.PID));
            // Update processes that already exist in the list
            Parallel.ForEach(Process.GetProcesses(), p =>
            {
                if (!currentIds.Remove(p.Id))
                {
                    _processes.Add(new ProcessModel(p));
                }
                else
                {
                    Parallel.ForEach(_processes.FindAll(x => x.PID == p.Id), proc =>
                    {
                        proc.Refresh();
                    });
                }
            });

            Parallel.ForEach(currentIds, id =>
            {
                _processes.RemoveAll(p => p.PID == id);
            });

            if (_refreshTimer.IsEnabled)
            {
                OnRequestTickData();
            }

            return this._processes;
        }
    }
}
