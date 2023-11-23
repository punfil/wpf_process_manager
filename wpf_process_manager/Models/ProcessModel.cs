using System;
using System.Diagnostics;
using System.Threading;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace wpf_process_manager.Models
{
    public class ProcessModel
    {
        private Process _process;
        public string Name { get; set; }
        public int PID { get; set; }
        public string CPUUsage { get; set; }
        public string MemoryUsage { get; set; }
        public string Priority { get; set; }
        public string ThreadsCount { get; set; }
        public int ParentID { get; set; }

        public ProcessModel(Process process)
        {
            this._process = process;
            try
            {
                this.PID = _process.Id;
                this.Name = _process.ProcessName;
                UpdateDataFields();
                this.ParentID = GetParentProcessID();
            }
            catch (Exception ex)
            {
                // Do nothing, because what to do?
                // Exception can only happen if the process finished (race condition) and we want to get data from it
                // It will be auto deleted during next Refresh() in ProcessManager
                _process = null; // Only possible action
            }
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

        public string GetThreadsCount()
        {
            var threadsCount = _process.Threads.Count;

            return $"Threads count: {threadsCount}";
        }

        public void Refresh()
        {
            _process.Refresh();
            this.UpdateDataFields();
        }

        private void UpdateDataFields()
        {
            try
            {
                this.ThreadsCount = GetThreadsCount();
                this.CPUUsage = GetCPUUsage();
                this.MemoryUsage = GetMemoryUsage();
                this.Priority = GetPriority();
            }
            catch (Exception ex)
            {
                // Do nothing, because what to do?
                // Exception can only happen if the process finished (race condition) and we want to get data from it
                // It will be auto deleted during next Refresh() in ProcessManager
                _process = null; // Only possible action
            }
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

        public bool SetPriority(ProcessPriorityModel priorityModel)
        {
            try
            {
                _process.PriorityClass = priorityModel.Priority;
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

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

            /* WA race condition between Refresh() and graceful app kill
             * tested with Notepad.exe */
            Thread.Sleep(500);
            return true;
        }

        private int GetParentProcessID()
        {
            try
            {
                return ParentProcessUtilities.GetParentProcess(_process.Handle);
            }
            catch (Exception ex) // Denied
            {
                return 0;
            }
        }
    }
}

[StructLayout(LayoutKind.Sequential)]
public struct ParentProcessUtilities
{
    internal IntPtr Reserved1;
    internal IntPtr PebBaseAddress;
    internal IntPtr Reserved2_0;
    internal IntPtr Reserved2_1;
    internal IntPtr UniqueProcessId;
    internal IntPtr InheritedFromUniqueProcessId;

    [DllImport("ntdll.dll")]
    private static extern int NtQueryInformationProcess(IntPtr processHandle, int processInformationClass, ref ParentProcessUtilities processInformation, int processInformationLength, out int returnLength);

    public static int GetParentProcess(IntPtr handle)
    {
        ParentProcessUtilities pbi = new ParentProcessUtilities();
        int returnLength;
        int status = NtQueryInformationProcess(handle, 0, ref pbi, Marshal.SizeOf(pbi), out returnLength);
        if (status != 0)
            throw new Win32Exception(status);
        try
        {
            return pbi.InheritedFromUniqueProcessId.ToInt32();
        }
        catch (ArgumentException)
        {
            return 0;
        }
    }
}