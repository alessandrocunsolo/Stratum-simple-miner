using Newtonsoft.Json;
using StratumMiner.StratumProcol.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StratumMiner.StratumProcol.Responses
{
    public class AuthorizeResponse :JsonRpcResponse
    {
        public bool Success { get; set; }

        public static AuthorizeResponse BuildFrom(JsonRpcResponse response)
        {
            var instance = new AuthorizeResponse();

            instance.Id = response.Id;
            instance.Error = response.Error;
            instance.Success = response.Result[0].ToObject<bool>();

            return instance;
        }
    }
}
