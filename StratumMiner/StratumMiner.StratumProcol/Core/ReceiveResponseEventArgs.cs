using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StratumMiner.StratumProcol.Core
{
    public class ReceiveResponseEventArgs : EventArgs
    {
        public ReceiveResponseEventArgs(byte[] data)
        {
            this.Data = data;
        }
        public byte[] Data { get; set; }
    }
}
