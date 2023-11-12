using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpf_process_manager.Models
{
    public class ProcessModel
    {
        private Process _process;

        public ProcessModel(Process process)
        {
            this._process = process;
        }

        public int GetPID()
        {
            return this._process.Id;
        }

        public string GetName()
        {
            return this._process.ProcessName;
        }

        public string GetCPUUsage()
        {
            double cpuUsage = (double)_process.TotalProcessorTime.TotalMilliseconds /
                (double)DateTime.Now.Subtract(_process.StartTime).TotalMilliseconds * 100.0;

            return cpuUsage.ToString();
        }

        public string GetMemoryUsage()
        {
            return _process.PagedMemorySize64.ToString();
        }

        public string GetPriority()
        {
            return _process.PriorityClass.ToString();
        }

        public string GetStartCommand()
        {
            var startInfo = _process.GetType().GetField("startInfo", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(_process) as ProcessStartInfo;
            
            return startInfo?.FileName + " " + startInfo?.Arguments;
        }

        public bool HasFinished()
        {
            return _process.HasExited;
        }

        public void Refresh()
        {
            this._process.Refresh();
        }
    }
}
