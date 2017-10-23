using StratumMiner.StratumProcol.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StratumMiner.StratumProcol.Responses
{
    public class ShareResponse : JsonRpcResponse
    {
        public ShareResponse(int? id)
        {
            this.Id = id;
        }
        public bool Valid { get; set; }

        public static ShareResponse BuildFrom(JsonRpcResponse response)
        {
            var instance = new ShareResponse(response.Id);
            instance.Error = response.Error;
            instance.Valid = bool.Parse(response.Result[0].ToString());
            return instance;
        }
    }
}
