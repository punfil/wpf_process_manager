using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
