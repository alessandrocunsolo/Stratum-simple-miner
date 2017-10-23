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
        void OpenConnection();

        void Send(JsonRpcRequest request);

        void Subscribe(SubscribeRequest request);
        void Authorize(AuthorizeRequest request);
        void Share(ShareRequest request);


        event EventHandler<ReceiveMessageEventArgs<SubscribeResponse>> OnSubscribeResponse;
        event EventHandler<ReceiveMessageEventArgs<NotifyRequest>> OnNotifyRequest;
        event EventHandler<ReceiveMessageEventArgs<SetDifficultyRequest>> OnSetDifficultyRequest;
        event EventHandler<ReceiveMessageEventArgs<AuthorizeResponse>> OnAuthorizeResponse;
        event EventHandler<ReceiveMessageEventArgs<ShareResponse>> OnShareResponse;
    }
}
