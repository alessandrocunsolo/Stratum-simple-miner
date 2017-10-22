using Newtonsoft.Json.Linq;
using StratumMiner.StratumProcol.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StratumMiner.StratumProcol.Requests
{
    public static class RequestExtensions
    {
        public static void Build(this AuthorizeRequest instance,string workername,string password )
        {
            instance.WorkerName = workername;
            instance.Password = password;
            if (instance.Params == null)
                instance.Params = new JArray();
            instance.Params.Add(workername);
            instance.Params.Add(password);
        }

        public static void Build(this ShareRequest request, string workerName, string jobId,string extraOnce2,string ntime,string nonce)
        {
            request.Params = new JArray();
            request.Params.Add(workerName);
            request.Params.Add(jobId);
            request.Params.Add(extraOnce2);
            request.Params.Add(ntime);
            request.Params.Add(nonce);
        }
    }
}
