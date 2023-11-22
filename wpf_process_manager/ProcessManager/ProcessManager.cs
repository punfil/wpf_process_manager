using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using wpf_process_manager.Models;

namespace wpf_process_manager.ProcessManager
{
    public class AutoRefreshReturn : EventArgs
    {
        public List<ProcessModel> processesList { get; set; }

        public AutoRefreshReturn(List<ProcessModel> processList)
        {
            this.processesList = processList;
        }
    }

    public class ProcessManager
    {
        private List<ProcessModel> _processes;
        private Timer _refreshTimer;
        public event EventHandler RefreshTimerHandler;
        public ProcessManager()
        {
            _processes = new List<ProcessModel>();
            _refreshTimer = null;
        }

        private Process[] GetRunningProcesses()
        {
            return Process.GetProcesses();

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

        private void OnRequestTickData(List<ProcessModel> processModels)
        {
            var ret = new AutoRefreshReturn(processModels);
            RefreshTimerHandler?.Invoke(this, ret);
        }

        public void AutoRefresh()
        {
            if (_refreshTimer == null)
            {
                _refreshTimer = new Timer(TimerCallback, null, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10));
            }
            else
            {
                _refreshTimer.Dispose();
                _refreshTimer = null;
            }
        }

        private void TimerCallback(object state)
        {
            Refresh();
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

            if (this._refreshTimer != null)
            {
                OnRequestTickData(_processes);
            }

            return this._processes;
        }
    }
}
