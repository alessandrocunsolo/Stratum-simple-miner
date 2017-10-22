using StratumMiner.StratumProcol.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StratumMiner.StratumProcol.Requests
{
    public class ShareRequest : JsonRpcRequest
    {
        public ShareRequest(int id)
        {
            this.Id = id;
            this.Method = "mining.submit"; 
        }
    }
}
