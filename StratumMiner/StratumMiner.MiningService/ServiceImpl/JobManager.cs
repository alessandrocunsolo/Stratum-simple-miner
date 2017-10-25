using StratumMiner.MiningService.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StratumMiner.MiningService.ServiceImpl
{
    public class JobManager
    {
        private List<Job> _jobs;
        private static JobManager instance = null;
        private bool _authorized = false;
        private bool _isworkerSubscribed = false;
        public event EventHandler<JobEventArgs> OnJobAdd;
        public event EventHandler<JobEventArgs> OnJobSuccess;


        protected JobManager()
        {
            this._jobs = new List<Job>();
        }

        public static JobManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new JobManager();
                return instance;
            }
        }

        public Job GetJobById(string id)
        {
            return this._jobs.Single(x => x.JobId == id);
        }

        public string ExtraNonce1 { get; set; }
        public int Difficulty { get; set; }

        public void AddJob(Job job)
        {
            lock(this)
            {
                this._jobs.Add(job);
                job.SetJobManager(this);
                this.RaiseOnJobAdd(job);
                job.Start();
                job.OnFoundNonce += (s, e) =>
                {
                    this.RaiseOnJobSuccess(e.Job);
                };
            }

        }

       

        public void ClearAllJob()
        {
            var runningJobs = this._jobs.Where(x => x.IsRunning).ToArray();
            foreach (var item in runningJobs)
            {
                item.Cancel();
            }
            this._jobs.RemoveAll(x => x.IsCancelled);
        }
        public bool IsAuthorized { get { return this._authorized; } }
        public bool IsWorkerSubscribed { get { return this._isworkerSubscribed; } }

        public void Authorize()
        {
            this._authorized = true;
        }
        public void SubscribeSuccess()
        {
            this._isworkerSubscribed = true;
        }

        private void RaiseOnJobAdd(Job job)
        {
            this.OnJobAdd?.Invoke(this, new JobEventArgs(job));
        }
        private void RaiseOnJobSuccess(Job job)
        {
            this.OnJobSuccess?.Invoke(this, new JobEventArgs(job));
        }


    }
}
