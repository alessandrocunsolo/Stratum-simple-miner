using Newtonsoft.Json;
using StratumMiner.StratumProcol.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StratumMiner.StratumProcol.Requests
{
    public class AuthorizeRequest : JsonRpcRequest
    {
        public AuthorizeRequest(int id)
        {
            this.Id = id;
            this.Method = "mining.authorize";
        }
        [JsonIgnore]
        public string WorkerName { get; set; }

        [JsonIgnore]
        public string Password { get; set; }


    }
}
