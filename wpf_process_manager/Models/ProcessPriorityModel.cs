using System.Diagnostics;

namespace wpf_process_manager.Models
{
    public class ProcessPriorityModel
    {
        public ProcessPriorityClass Priority { get; }
        public string Name { get; }

        public ProcessPriorityModel(ProcessPriorityClass priority)
        {
            this.Priority = priority;
            this.Name = priority.ToString();
        }
    }
}
