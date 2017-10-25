using StratumMiner.StratumProcol.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StratumMiner.StratumProcol.Core;
using System.Net.Sockets;
using Newtonsoft.Json;
using StratumMiner.StratumProcol.Requests;
using StratumMiner.StratumProcol.Responses;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Collections;

namespace StratumMiner.StratumProcol.ServiceImpl
{
    public class SlushMiningPoolClient : IMiningPoolClient
    {
        private Uri _uri;
        private Socket _socket;
        private Queue<string> _queue;
        private Task _queueTask;
        private object synObject = new object();
        public event EventHandler<string> OnEnqueMessage;
        public event EventHandler<ReceiveMessageEventArgs<SubscribeResponse>> OnSubscribeResponse;
        public event EventHandler<ReceiveMessageEventArgs<NotifyRequest>> OnNotifyRequest;
        public event EventHandler<ReceiveMessageEventArgs<SetDifficultyRequest>> OnSetDifficultyRequest;
        public event EventHandler<ReceiveMessageEventArgs<AuthorizeResponse>> OnAuthorizeResponse;
        public event EventHandler<ReceiveMessageEventArgs<ShareResponse>> OnShareResponse;

        private Hashtable _requestTable;

        public bool _stopReceive;
        public SlushMiningPoolClient(Uri uri)
        {
            this._uri = uri;
            this._queue = new Queue<string>();
            this._requestTable = new Hashtable();
        }

        public void OpenConnection()
        {
            this._socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            _socket.Connect(_uri.Host, _uri.Port);

            this.OnEnqueMessage += SlushMiningPoolClient_OnEnqueMessage;

            this._queueTask = Task.Factory.StartNew(() =>
           {
               this._stopReceive = false;
               var buffer = new byte[16384];
               while (!_stopReceive)
               {
                   var size = _socket.Receive(buffer);
                   var validBuffer = buffer;
                   if (size < buffer.Length)
                   {
                       validBuffer = validBuffer.Take(size).ToArray();
                   }

                   var str = System.Text.Encoding.UTF8.GetString(validBuffer);
                   var messages = str.Split('\n').Where(m => !string.IsNullOrWhiteSpace(m)).ToArray();
                   //if (!string.IsNullOrWhiteSpace(messages.Last()))
                   //{

                   //}
                   foreach (var item in messages)
                   {
                       lock (this.synObject)
                       {
                           this._queue.Enqueue(item);
                       }

                       this.OnEnqueMessage(this, item);
                   }


               }


           });



        }



        public void Send(JsonRpcRequest request)
        {
            var jsonString = JsonConvert.SerializeObject(request);
            var requestText = jsonString + "\n";
            var bytes = System.Text.Encoding.UTF8.GetBytes(requestText);
            var number = _socket.Send(bytes);
        }

        public void Authorize(AuthorizeRequest request)
        {
            if (!request.Id.HasValue) throw new ArgumentNullException(nameof(request.Id));
            this._requestTable.Add(request.Id.Value.ToString(), request);
            this.Send(request);
        }

        public void Subscribe(SubscribeRequest request)
        {
            if (!request.Id.HasValue) throw new ArgumentNullException(nameof(request.Id));
            this._requestTable.Add(request.Id.Value.ToString(), request);
            this.Send(request);
        }

        public void Share(ShareRequest request)
        {
            if (!request.Id.HasValue) throw new ArgumentNullException(nameof(request.Id));
            this._requestTable.Add(request.Id.Value.ToString(), request);
            this.Send(request);
        }


        private void SlushMiningPoolClient_OnEnqueMessage(object sender, string e)
        {
            Task.Factory.StartNew(() =>
           {
               var str = "";
               lock (this.synObject)
               {
                   str = this._queue.Dequeue();
               }
               var jObject = JObject.Parse(str);
               var id = jObject["id"];
               if (id != null && this._requestTable.ContainsKey(id.ToString()))
               {
                   var request = this._requestTable[id.ToString()];
                   var reponse = JsonConvert.DeserializeObject<JsonRpcResponse>(str);
                   if (request is AuthorizeRequest)
                   {
                       var authResponse = AuthorizeResponse.BuildFrom(reponse);
                       this.RaiseOnAuthorizeResponse(new ReceiveMessageEventArgs<AuthorizeResponse>(authResponse));
                   }
                   if (request is SubscribeRequest)
                   {
                       var subScribeResponse = SubscribeResponse.BuildFrom(reponse);
                       this.RaiseOnSubscribeResponse(new ReceiveMessageEventArgs<SubscribeResponse>(subScribeResponse));
                   }
                   if (request is ShareRequest)
                   {
                       var shareResponse = ShareResponse.BuildFrom(reponse);
                       this.RaiseOnShareResponse(new ReceiveMessageEventArgs<ShareResponse>(shareResponse));
                   }
               }
               else
               {
                   var method = jObject["method"].ToString();
                   if (!string.IsNullOrWhiteSpace(method))
                   {
                       if (method == "mining.notify")
                       {
                           var jsonRequest = JsonConvert.DeserializeObject<JsonRpcRequest>(str);
                           var request = NotifyRequest.BuildFrom(jsonRequest);
                           this.RaiseOnNotifyRequest(new ReceiveMessageEventArgs<NotifyRequest>(request));
                       }
                       if (method == "mining.set_difficulty")
                       {
                           var jsonRequest = JsonConvert.DeserializeObject<JsonRpcRequest>(str);
                           var request = SetDifficultyRequest.BuildFrom(jsonRequest);
                           this.RaiseOnSetDifficultyRequest(new ReceiveMessageEventArgs<SetDifficultyRequest>(request));
                       }
                   }
               }


           });

        }



        #region Event Raiser

        private void RaiseOnEnqueMessage(string e)
        {
            this.OnEnqueMessage?.Invoke(this, e);
        }


        private void RaiseOnSubscribeResponse(ReceiveMessageEventArgs<SubscribeResponse> e)
        {
            this.OnSubscribeResponse?.Invoke(this, e);
        }
        private void RaiseOnNotifyRequest(ReceiveMessageEventArgs<NotifyRequest> e)
        {
            this.OnNotifyRequest?.Invoke(this, e);
        }

        private void RaiseOnSetDifficultyRequest(ReceiveMessageEventArgs<SetDifficultyRequest> e)
        {
            this.OnSetDifficultyRequest?.Invoke(this, e);
        }

        private void RaiseOnAuthorizeResponse(ReceiveMessageEventArgs<AuthorizeResponse> e)
        {
            this.OnAuthorizeResponse?.Invoke(this, e);
        }
        private void RaiseOnShareResponse(ReceiveMessageEventArgs<ShareResponse> e)
        {
            this.OnShareResponse?.Invoke(this, e);
        }


        #endregion






    }
}
