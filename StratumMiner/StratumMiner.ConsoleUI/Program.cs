using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StratumMiner.MiningService.ServiceImpl;
using StratumMiner.StratumProcol.Core;
using StratumMiner.StratumProcol.Requests;
using StratumMiner.StratumProcol.Responses;
using StratumMiner.StratumProcol.ServiceImpl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace StratumMiner.ConsoleUI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            //socket.Connect("eu.stratum.slushpool.com", 3333);
            //var jsonString = "{\"id\": 1, \"method\": \"mining.subscribe\", \"params\": []}\n";
           
            var miningService = new MiningService.ServiceImpl.MiningService();
            

            var uri = new Uri("stratum+tcp://eu.stratum.slushpool.com:3333");
            var client = new SlushMiningPoolClient(uri);
            SubscribeResponse subscribeResponse = null;
            var hexString = "1234567890abcdef";
            var requestIndex = 0;
            var syncObject = new object();
            client.OpenConnection();

            client.OnReceiveMessage += (s, e) =>
             {
                 Task.Factory.StartNew(() =>
                 {
                     var str = System.Text.Encoding.UTF8.GetString(e.Data);
                     Console.WriteLine(str);
                     var parts = str.Split('\n').Where(x => !string.IsNullOrEmpty(x)).ToArray();
                     foreach (var item in parts)
                     {
                         var jObject = JObject.Parse(item);
                         if (jObject["method"] != null && jObject["method"].ToString() == "mining.notify")
                         {
                             ShareRequest shareRequest2;
                             lock (syncObject)
                             {
                                 shareRequest2 = new ShareRequest(++requestIndex);
                             }

                             var requestRpc = JsonConvert.DeserializeObject<JsonRpcRequest>(item);
                             var request = NotifyRequest.BuildFrom(requestRpc);
                             var random = new Random();
                             var extraOnce2 = new String(hexString.OrderBy(x => random.Next()).Take(8).ToArray());

                             var nonce = miningService.FindOnce(miningService.BuildBlockWithoutOnce(subscribeResponse, request, extraOnce2), 4, int.MaxValue);

                             if (!string.IsNullOrWhiteSpace(nonce))
                             {
                                 shareRequest2.Build("acunsolo.acunsolo-worker2", request.JobId, extraOnce2, request.NTime, nonce);
                                 var result2 = client.Send(shareRequest2);
                                 Console.WriteLine("Result = {0}", result2.Result);
                             }
                         }
                     }

                 });
             };
            SubscribeRequest obj;
            lock (syncObject)
            {
                obj = new SubscribeRequest(++requestIndex);
            }
            subscribeResponse = client.Subscribe(obj);
            AuthorizeRequest authRequest;
            lock (syncObject)
            {
                authRequest = new AuthorizeRequest(++requestIndex);
            }
            authRequest.Build("acunsolo.acunsolo-worker2", "foo");
            var authorizeResponse = client.Authorize(authRequest);
            //ShareRequest shareRequest = null;
            //lock (syncObject)
            //{
            //    shareRequest = new ShareRequest(++requestIndex);
            //}
            //shareRequest.Build("acunsolo.acunsolo-worker2", subscribeResponse.Notify.JobId, extraOnce22, subscribeResponse.Notify.NTime, nonce);
            //var result = client.Send(shareRequest);
            //Console.WriteLine("Result = {0}", result.Result);
            Console.ReadKey();
        }
    }
}
