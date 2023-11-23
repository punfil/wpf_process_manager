using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

        public bool KillProcess(ProcessModel process)
        {
            if (process != null && !process.HasFinished())
            {
                return process.Kill();
            }

            return false;
        }

        public bool SetProcessPriority(ProcessModel process, ProcessPriorityModel priority)
        {
            if (process != null && !process.HasFinished() && priority != null)
            {
                return process.SetPriority(priority);
            }

            return false;
        }

        public Dictionary<string, string> GetProcessDetails(ProcessModel process)
        {
            if (process != null)
            {
                return process.GetDetails();
            }

            return null;
        }

        static bool CompareWithPredicate(double value1, double value2, string predicate)
        {
            switch (predicate)
            {
                case "<=":
                    return value1 <= value2;
                case "<":
                    return value1 < value2;
                case ">=":
                    return value1 >= value2;
                case ">":
                    return value1 > value2;
                case "==":
                    return value1 == value2;
                default:
                    throw new ArgumentException("Invalid predicate");
            }
        }

        private void RemoveAllFrom_processes(List<ProcessModel> list, List<ProcessModel> toBeRemoved)
        {
            foreach (ProcessModel remove in toBeRemoved)
            {
                list.Remove(remove);
            }

            toBeRemoved.Clear();
        }

        public List<ProcessModel> FilterProcesses(string nameFilter, string cpuUsageFilter, string memoryUsageFilter, string priorityFilter)
        {
            List<ProcessModel> filteredProcesses = new List<ProcessModel>(_processes);
            
            List<ProcessModel> toBeRemoved = new List<ProcessModel>();
            if (nameFilter != null && nameFilter != "")
            {
                foreach (ProcessModel process in filteredProcesses)
                {
                    if (!process.Name.Contains(nameFilter))
                    {
                        toBeRemoved.Add(process);
                    }
                }
            }
            RemoveAllFrom_processes(filteredProcesses, toBeRemoved);

            if (cpuUsageFilter != null && cpuUsageFilter != "")
            {
                var cpuFilters = cpuUsageFilter.Split(' ');
                if (cpuFilters.Length < 2)
                {
                    return filteredProcesses;
                }

                var predicate = cpuFilters[0];
                var filterValue = double.Parse(cpuFilters[1]);
                foreach (ProcessModel process in filteredProcesses)
                {
                    // Remove " %"
                    if (!CompareWithPredicate(double.Parse(process.CPUUsage.Remove(process.CPUUsage.Length-2)), filterValue, predicate))
                    {
                        toBeRemoved.Add(process);
                    }
                }
            }
            RemoveAllFrom_processes(filteredProcesses, toBeRemoved);

            if (memoryUsageFilter != null && memoryUsageFilter != "")
            {
                var memoryFilters = memoryUsageFilter.Split(' ');
                if (memoryFilters.Length < 2)
                {
                    return filteredProcesses;
                }

                var predicate = memoryFilters[0];
                var filterValue = double.Parse(memoryFilters[1]);
                foreach (ProcessModel process in filteredProcesses)
                {
                    // Remove " MB"
                    if (!CompareWithPredicate(double.Parse(process.MemoryUsage.Remove(process.MemoryUsage.Length-3)), filterValue, predicate))
                    {
                        toBeRemoved.Add(process);
                    }
                }
            }
            RemoveAllFrom_processes(filteredProcesses, toBeRemoved);

            if (priorityFilter != null && priorityFilter != "")
            {
                foreach (ProcessModel process in filteredProcesses)
                {
                    if (process.Priority == priorityFilter)
                    {
                        toBeRemoved.Add(process);
                    }
                }
            }
            RemoveAllFrom_processes(filteredProcesses, toBeRemoved);

            return filteredProcesses;
        }

        private void OnRequestTickData(List<ProcessModel> processModels)
        {
            var ret = new AutoRefreshReturn(processModels);
            RefreshTimerHandler?.Invoke(this, ret);
        }

        public void AutoRefresh(int intervalValue)
        {
            // Due to long time of Refresh() execution, interval value must be greater than 8 seconds.
            int timerInterval = int.Max(8, intervalValue);
            if (_refreshTimer == null)
            {
                _refreshTimer = new Timer(TimerCallback, null, TimeSpan.FromSeconds(timerInterval), TimeSpan.FromSeconds(timerInterval));
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
