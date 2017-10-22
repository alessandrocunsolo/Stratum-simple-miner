using StratumMiner.StratumProcol.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StratumMiner.StratumProcol.Requests
{
    public class SetDifficultyRequest: JsonRpcRequest
    {
        public int Difficulty { get; set; }
    }
}
