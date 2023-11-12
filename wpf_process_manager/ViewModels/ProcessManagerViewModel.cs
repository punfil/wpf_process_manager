using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wpf_process_manager.ProcessManager;

namespace wpf_process_manager.ViewModels
{
    public class ProcessManagerViewModel
    {
        private ProcessManager.ProcessManager _processManager;
        public ProcessManagerViewModel()
        {
            _processManager = new ProcessManager.ProcessManager();
        }
    }
}
