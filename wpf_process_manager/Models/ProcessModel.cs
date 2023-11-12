using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wpf_process_manager.ProcessManager;

namespace wpf_process_manager.Models
{
    public class ProcessModel
    {
        private Process _process;
        public string Name { get; set; }
        public int PID { get; set; }
        public string Command { get; set; }
        public string CPUUsage { get; set; }
        public string MemoryUsage { get; set; }
        public string Priority { get; set; }

        public ProcessModel(Process process)
        {
            this._process = process;
            UpdateDataFields();
        }
        public string GetCPUUsage()
        {
            try
            {
                return Math.Round((double)_process.TotalProcessorTime.TotalMilliseconds /
                    (double)DateTime.Now.Subtract(_process.StartTime).TotalMilliseconds * 100.0, 2).ToString() + " %";
            }
            catch (Exception ex)
            {
                return 0.0.ToString() + " %";
            }
        }

        public string GetStartCommand()
        {
            //var startInfo = _process.GetType().GetField("startInfo", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(_process) as ProcessStartInfo;

            //return startInfo?.FileName + " " + startInfo?.Arguments;
            return "Unknown";
        }

        public void Refresh()
        {
            _process.Refresh();
        }

        private void UpdateDataFields()
        {
            this.PID = _process.Id;
            this.Name = _process.ProcessName;
            this.Command = GetStartCommand();
            this.CPUUsage = GetCPUUsage();
            this.MemoryUsage = GetMemoryUsage();
            this.Priority = GetPriority();
        }

        private string GetPriority()
        {
            try
            {
                return _process.PriorityClass.ToString();
            }
            catch (Exception ex)
            {
                return "Unknown";
            }
        }

        /// <summary>
        /// Get Memory Usage of the process
        /// </summary>
        /// <returns> Memory usage in megabytes. </returns>
        private string GetMemoryUsage()
        {
            try
            {
                return Math.Round(_process.PagedMemorySize64/1024.0/1024.0, 2).ToString() + " MB";
            }
            catch (Exception ex)
            {
                return "Unknown";
            }
        }

        public bool HasFinished()
        {
            try
            {
                return _process.HasExited;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Kill()
        {
            try
            {
                _process.Kill();
            }
            catch (Exception exc)
            {
                return false;
            }

            return true;
        }
    }
}
