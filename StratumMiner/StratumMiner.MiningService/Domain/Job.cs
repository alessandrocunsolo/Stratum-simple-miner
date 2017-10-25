using StratumMiner.MiningService.Helpers;
using StratumMiner.MiningService.ServiceImpl;
using StratumMiner.StratumProcol.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StratumMiner.MiningService.Domain
{
    public class Job
    {
        private Task jobTask;
        private bool isRunning = false;
        private bool _cancelled = false;
        
        private JobManager jobManager;

        public event EventHandler<JobEventArgs> OnFoundNonce;
        public Job(string jobId)
        {
            this.JobId = jobId;
        }

        public void SetJobManager(JobManager jobManager)
        {
            this.jobManager = jobManager;
        }

        public string JobId { get; private set; }
        public string[] MerkleBranch { get;set;}
        public string NBits { get; set; }
        public string NTime { get; set; }
        public string PreviousHash { get; set; }
        public string Version { get;  set; }
        public string Coinbase1 { get; set; }
        public string Coinbase2 { get; set; }
        public UInt256 TargetDiff { get; set; }
        public string FoundNonce { get; set; }
        public string ExtraNonce2 { get; set; }

        public static Job CreateFromRequest(NotifyRequest request)
        {
            var instance = new Job(request.JobId);
            instance.MerkleBranch = request.MerkleBranch;
            instance.NBits = request.NBits;
            instance.NTime = request.NTime;
            instance.PreviousHash = request.PreviousHash;
            instance.Version = request.Version;
            instance.Coinbase1 = request.Coinbase1;
            instance.Coinbase2 = request.Coinbase2;

            instance.TargetDiff = UInt256.Parse(instance.NBits);

            return instance;
        }


        public bool IsRunning { get { return this.isRunning; } }
        public void Start()
        {
           this.jobTask =  Task.Factory.StartNew(() =>
           {
               this.isRunning = true;
               while(isRunning)
               {
                   var nonce = "";
                   var extranonce1 = this.jobManager.ExtraNonce1;
                   var difficulty = this.jobManager.Difficulty;
                   for (UInt32 i = 0; i < UInt32.MaxValue; i++)
                   {
                       this.ExtraNonce2 = MiningHelper.ConvertByteToString(BitConverter.GetBytes(i));
                      
                       nonce = MiningHelper.FindNonce(MiningHelper.BuildBlockWithoutOnce(extranonce1, this, this.ExtraNonce2),difficulty, UInt32.MaxValue,this.TargetDiff);
                       if (!string.IsNullOrWhiteSpace(nonce)) break;
                   }

                   if (!string.IsNullOrWhiteSpace(nonce))
                   {
                       this.FoundNonce = nonce;
                       this.RainseOnFoundNonce(this);
                       
                   }
               }
           });

        }

        public bool IsCancelled { get { return this._cancelled; } }

        public void Cancel()
        {
            this.isRunning = false;
            this._cancelled = true;
            //this.jobTask.Dispose();
        }


        private void RainseOnFoundNonce(Job j)
        {
            this.OnFoundNonce?.Invoke(this, new JobEventArgs(j));
        }
    }
}
