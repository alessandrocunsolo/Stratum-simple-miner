using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace StratumMiner.StratumProcol.Core
{
    public class JsonRpcRequest : JsonRpcBase
    {
        [JsonProperty("method")]
        public string Method { get; set; }
        [JsonProperty("params")]
        public JArray Params { get; set; }
    }
}
