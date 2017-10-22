using StratumMiner.StratumProcol.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StratumMiner.StratumProcol.Responses
{
    /*job_id - ID of the job. Use this ID while submitting share generated from this job.
      prevhash - Hash of previous block.
      coinb1 - Initial part of coinbase transaction.
      coinb2 - Final part of coinbase transaction.
      merkle_branch - List of hashes, will be used for calculation of merkle root. This is not a list of all transactions, it only contains prepared hashes of steps of merkle tree algorithm. Please read some materials for understanding how merkle trees calculation works. Unfortunately this example don't have any step hashes included, my bad!
      version - Bitcoin block version.
      nbits - Encoded current network difficulty
      ntime - Current ntime/
      clean_jobs - When true, server indicates that submitting shares from previous jobs don't have a sense and such shares will be rejected. When this flag is set, miner should also drop all previous jobs, so job_ids can be eventually rotated.
    */
    public class NotifyRequest : JsonRpcRequest
    {
        public string JobId { get; set; }
        public string PreviousHash { get; set; }
        public string Coinbase1 { get; set; }
        public string Coinbase2 { get; set; }
        public string[] MerkleBranch { get; set; }
        public string Version { get; set; }
        public string NBits { get; set; }
        public string NTime { get; set; }
        public bool CleanJobs { get; set; }

        public static NotifyRequest BuildFrom(JsonRpcRequest request)
        {
            
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
            return notifyResponse;
        }
        
    }
}
