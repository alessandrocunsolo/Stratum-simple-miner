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

namespace StratumMiner.StratumProcol.ServiceImpl
{
    public class SlushMiningPoolClient : IMiningPoolClient
    {
        private Uri _uri;
        private Socket _socket;

        public event EventHandler<ReceiveResponseEventArgs> OnReceiveMessage;

        public SlushMiningPoolClient(Uri uri)
        {
            this._uri = uri;
        }

        public void OpenConnection()
        {
            this._socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            _socket.Connect(_uri.Host, _uri.Port);
        }

        public JsonRpcResponse Send (JsonRpcRequest request)
        {
            var str = this.GetResponse(request);
            var result = str.Split('\n');
            var responseObj = JsonConvert.DeserializeObject<JsonRpcResponse>(result[0]);
            return responseObj;
        }


        

        private string GetResponse(JsonRpcRequest request)
        {
            var jsonString = JsonConvert.SerializeObject(request);
            var requestText = jsonString + "\n";
            var bytes = System.Text.Encoding.UTF8.GetBytes(requestText);
            var number = _socket.Send(bytes);
            var dataSize = _socket.Available;
            while(dataSize == 0)
            {
                Thread.Sleep(100);
                dataSize = _socket.Available;
            }
            var buffer = new byte[dataSize];
            var size = _socket.Receive(buffer);
            var str = System.Text.Encoding.UTF8.GetString(buffer);
            return str;
        }

        public SubscribeResponse Subscribe(SubscribeRequest request)
        {
            var str = this.GetResponse(request);
            Task.Factory.StartNew(() =>
           {
               var tempBuffer = new List<Byte>();
               while (true)
               {
                   
                   var buffer = new byte[2048];
                   var size = this._socket.Receive(buffer);
                   if (size <= buffer.Length)
                   {
                       if (tempBuffer.Count > 0)
                       {
                           this.RaiseOnReceiveMessage(new ReceiveResponseEventArgs(new List<Byte>(tempBuffer).ToArray()));
                           tempBuffer.Clear();
                       }
                       else
                       {
                           this.RaiseOnReceiveMessage(new ReceiveResponseEventArgs(buffer.Take(size).ToArray()));
                       }
                       
                   }
                   else
                   {
                       tempBuffer.AddRange(buffer);
                   }
                   
               }
           });
            return SubscribeResponse.BuildFrom(str);
        }

        public void RaiseOnReceiveMessage(ReceiveResponseEventArgs e)
        {
            this.OnReceiveMessage?.Invoke(this, e);
        }

        public AuthorizeResponse Authorize(AuthorizeRequest request)
        {
            var response = this.Send(request);
            return AuthorizeResponse.BuildFrom(response);
        }
    }
}
