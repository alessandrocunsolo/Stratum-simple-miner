using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StratumMiner.StratumProcol.Core
{
    public class ReceiveMessageEventArgs<T> : EventArgs
    {
        public ReceiveMessageEventArgs(T item)
        {
            this.Message = item;
        }

        public T Message { get; private set; }
    }
}
