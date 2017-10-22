using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StratumMiner.MiningService.ServiceImpl;
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
            var obj = new SubscribeRequest(1);
            var miningService = new MiningService.ServiceImpl.MiningService();
            

            var uri = new Uri("stratum+tcp://eu.stratum.slushpool.com:3333");
            var client = new SlushMiningPoolClient(uri);
            client.OpenConnection();
           
            var response = client.Subscribe(obj);
            

            var nonce = miningService.ConvertByteToString(miningService.BuildNonce(miningService.BuildBlockHeader(response)));
            var authRequest = new AuthorizeRequest(2);
            authRequest.Build("acunsolo.acunsolo-worker2", "foo");
            var authorizeResponse = client.Authorize(authRequest);
            var shareRequest = new ShareRequest(3);
            shareRequest.Build("acunsolo.acunsolo - worker2", response.Notify.JobId, "00000000", response.Notify.NTime, nonce);
            var result = client.Send(shareRequest);

        }
    }
}
