using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace StratumMiner.StratumProcol.Core
{
    public class JsonRpcBase
    {
        [JsonProperty("id")]
        public int? Id { get; set; }
    }
}
