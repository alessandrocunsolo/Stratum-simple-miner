using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StratumMiner.MiningService.Domain;
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
           
            var uri = new Uri("stratum+tcp://eu.stratum.slushpool.com:3333");
            var client = new SlushMiningPoolClient(uri);
           
            client.OnEnqueMessage += (s, e) =>
              {
                  Console.WriteLine(e);
              };
            client.OpenConnection();


            client.Subscribe(new SubscribeRequest(1));

            var authRequest = new AuthorizeRequest(2);
            authRequest.Build("acunsolo.acunsolo-worker2", "foo");

            var extranonce1 = "";

            var jobManager = JobManager.Instance;

            jobManager.OnJobSuccess += (s, e) =>
                 {
                     var shareRequest = new ShareRequest(GetRequestIndex());
                     shareRequest.Build("acunsolo.acunsolo-worker2", e.Job.JobId, e.Job.ExtraNonce2, e.Job.NTime, e.Job.FoundNonce);
                     client.Share(shareRequest);
                 };

            client.OnSubscribeResponse += (s, e) =>
            {
                var response = e.Message;
                extranonce1 = response.Extranonce1;
                JobManager.Instance.ExtraNonce1 = extranonce1;
                JobManager.Instance.SubscribeSuccess();
            };

            client.OnNotifyRequest += (s, e) =>
            {
                Task.Factory.StartNew(() =>
                {
                    if (e.Message.CleanJobs)
                        JobManager.Instance.ClearAllJob();
                    if (JobManager.Instance.IsWorkerSubscribed)
                    {
                        var job = Job.CreateFromRequest(e.Message);
                        JobManager.Instance.AddJob(job);
                    }
                    
                });
            };
            client.OnSetDifficultyRequest += (s, e) =>
             {
                 var message = e.Message;
                 JobManager.Instance.Difficulty = message.Difficulty;
             };
            client.OnAuthorizeResponse += (s, e) =>
              {
                  JobManager.Instance.Authorize();
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
