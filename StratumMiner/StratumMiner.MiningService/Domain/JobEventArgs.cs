using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StratumMiner.MiningService.Domain
{
    public class JobEventArgs : EventArgs
    {
        public JobEventArgs(Job job)
        {
            this.Job = job;
        }

        public Job Job { get; private set; }
    }
}
