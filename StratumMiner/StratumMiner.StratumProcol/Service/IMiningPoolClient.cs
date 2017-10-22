using StratumMiner.StratumProcol.Core;
using StratumMiner.StratumProcol.Requests;
using StratumMiner.StratumProcol.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StratumMiner.StratumProcol.Service
{
    public interface IMiningPoolClient
    {
        JsonRpcResponse Send(JsonRpcRequest request);

        SubscribeResponse Subscribe(SubscribeRequest request);

        AuthorizeResponse Authorize(AuthorizeRequest request);
    }
}
