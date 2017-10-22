using Newtonsoft.Json;
using StratumMiner.StratumProcol.Core;
using StratumMiner.StratumProcol.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StratumMiner.StratumProcol.Responses
{
    public class SubscribeResponse : JsonRpcResponse
    {
        public SubscribeResponse(int id)
        {
            this.Id = id;
        }
     

        public string Extranonce1 { get; private set; }

        public int Extranonce2Size { get; private set; }

        public NotifyRequest Notify { get; set; }
        

        public SetDifficultyRequest SetDifficulty { get; set; }
        
        public static SubscribeResponse BuildFrom(string responseText)
        {
            var parts = responseText.Split('\n');
            var jsonResponse = JsonConvert.DeserializeObject<JsonRpcResponse>(parts[0]);

            var instance = new SubscribeResponse(jsonResponse.Id.Value);
            instance.Extranonce1 = jsonResponse.Result[1].ToString();
            instance.Extranonce2Size =int.Parse(jsonResponse.Result[2].ToString());

            var otherJsonResponse = parts.Skip(1).Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => JsonConvert.DeserializeObject<JsonRpcRequest>(x))
                .ToArray();

            //Notify Response
            
            if (otherJsonResponse.Any(x => x.Method == "mining.notify"))
            {
                var request = otherJsonResponse.First(x => x.Method == "mining.notify");
                var notifyResponse = new NotifyRequest();
                notifyResponse.JobId = request.Params[0].ToString();
                notifyResponse.PreviousHash = request.Params[1].ToString();
                notifyResponse.Coinbase1 = request.Params[2].ToString();
                notifyResponse.Coinbase2 = request.Params[3].ToString();
                notifyResponse.MerkleBranch = request.Params[4].ToObject<string[]>();
                notifyResponse.Version = request.Params[5].ToString();
                notifyResponse.NBits = request.Params[6].ToString();
                notifyResponse.NTime = request.Params[7].ToString();
                notifyResponse.CleanJobs = request.Params[8].ToObject<bool>();
                instance.Notify = notifyResponse;

            }
            if (otherJsonResponse.Any(x => x.Method == "mining.set_difficulty"))
            {
                var request = otherJsonResponse.First(x => x.Method == "mining.set_difficulty");
                var setDifficultyRequest = new SetDifficultyRequest();
                setDifficultyRequest.Difficulty = request.Params[0].ToObject<int>();
                instance.SetDifficulty = setDifficultyRequest;
            }

            return instance;





        }

    }
}
