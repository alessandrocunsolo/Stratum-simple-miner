using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace StratumMiner.StratumProcol.Core
{
    public class JsonRpcResponse : JsonRpcBase
    {
        [JsonProperty("result")]
        public JArray Result { get; set; }
        [JsonProperty("error")]
        public string Error { get; set; }
     
    }
}
