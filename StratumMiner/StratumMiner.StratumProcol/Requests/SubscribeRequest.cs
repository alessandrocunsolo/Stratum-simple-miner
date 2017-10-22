using Newtonsoft.Json.Linq;
using StratumMiner.StratumProcol.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace StratumMiner.StratumProcol.Requests
{
    public class SubscribeRequest : JsonRpcRequest
    {
        public SubscribeRequest(int id)
        {
            this.Method = "mining.subscribe";
            this.Id = id;
            this.Params = new JArray();
        }


    }
}
