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
        private static object syncRoot = new object();
        private static int requestIndex = 3;
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


            client.OnEnqueMessage += (s, e) =>
              {
                  Console.WriteLine(e);
              };
            client.OpenConnection();


            client.Subscribe(new SubscribeRequest(1));

            var authRequest = new AuthorizeRequest(2);
            authRequest.Build("acunsolo.acunsolo-worker2", "foo");

            var extranonce1 = "";

            client.OnSubscribeResponse += (s, e) =>
            {
                var response = e.Message;
                extranonce1 = response.Extranonce1;
            };

            client.OnNotifyRequest += (s, e) =>
            {
                Task.Factory.StartNew(() =>
                {
                    var request = e.Message;
                    var nonce = "";
                    string extraOnce2 = "";
                    for (UInt32 i = 0; i < UInt32.MaxValue; i++)
                    {
                        extraOnce2 = miningService.ConvertByteToString(BitConverter.GetBytes(i));
                        nonce = miningService.FindNonce(miningService.BuildBlockWithoutOnce(extranonce1, request, extraOnce2), 4, UInt32.MaxValue);
                        if (!string.IsNullOrWhiteSpace(nonce)) break;
                    }

                    if (!string.IsNullOrWhiteSpace(nonce))
                    {
                        var shareRequest = new ShareRequest(GetRequestIndex());
                        shareRequest.Build("acunsolo.acunsolo-worker2", request.JobId, extraOnce2, request.NTime, nonce);
                        client.Share(shareRequest);
                    }
                    
                });
            };



           
            Console.ReadKey();
        }

        public static int GetRequestIndex()
        {
            lock(syncRoot)
            {
                requestIndex += 1;
                return requestIndex;
            }
        }




    }
}
