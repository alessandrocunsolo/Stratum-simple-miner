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
                instance.Notify = NotifyRequest.BuildFrom(request);

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
