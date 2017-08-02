using System;
using System.Threading;
using System.Threading.Tasks;

namespace BusterWood.Serverless
{
    abstract class EssentialJobEngine
    {
        private Task jobRunner;
        private CancellationTokenSource cancelSource;

        public virtual void Start()
        {
            CleanUpPreviouslyClaimedJobs();
            cancelSource = new CancellationTokenSource();
            jobRunner = Run();
        }

        /// <summary>
        /// Clean up the state of jobs that were running but the process was stopped unexpectedly (killed or crashed)
        /// </summary>
        protected abstract void CleanUpPreviouslyClaimedJobs();

        public virtual void Stop()
        {
            cancelSource.Cancel();
            jobRunner?.Wait();
            WaitForAllRunningJobsToFinish();
            jobRunner = null;
        }

        protected abstract void WaitForAllRunningJobsToFinish();

        /// <summary>
        /// start jobs in a loop
        /// </summary>
        protected virtual async Task Run()
        {
            await Task.Yield();  // force this method to run asynchronously

            var cancel = cancelSource.Token;
            while (!cancel.IsCancellationRequested)
            {
                var job = await ClaimJob(cancel);
                if (job == null)
                    break; // we have been cancelled
                var dontWait = TryToRunJob(job);
            }            
        }

        /// <summary>
        /// Claim the next job that can be run by this engine
        /// </summary>
        internal abstract Task<JobData> ClaimJob(CancellationToken cancel);

        /// <summary>
        /// no need for retrying as it will be handled by the job failing and being re-claimed
        /// </summary>
        protected virtual async Task TryToRunJob(JobData job)
        {
            try
            {
                await RunJob(job);
                JobSucceeded(job);
            }
            catch (Exception ex)
            {
                JobFailed(job, ex);
            }
        }

        internal abstract void JobSucceeded(JobData job);

        /// <summary>
        /// Run the job in process, out of process, in a new process, reuse a process, etc
        /// </summary>
        protected abstract Task RunJob(JobData job);

        /// <summary>
        /// Record the failure of a job
        /// </summary>
        protected abstract void JobFailed(JobData job, Exception ex);
    }

    class JobEngine : EssentialJobEngine
    {
        readonly JobClaimer jobClaimer;
        readonly JobRunner jobRunner;
        readonly JobPersister jobPersister;

        public JobEngine(JobClaimer jobClaimer, JobRunner jobRunner, JobPersister jobPersister)
        {
            this.jobClaimer = jobClaimer;
            this.jobRunner = jobRunner;
            this.jobPersister = jobPersister;
        }

        internal override async Task<JobData> ClaimJob(CancellationToken cancel)
        {
            StdErr.Info($"waiting for next job");
            var job = await jobClaimer.ClaimNext(cancel);
            if (job != null)
                StdErr.Info($"claimed job {job}");
            return job;
        }

        protected override Task RunJob(JobData job)
        {
            StdErr.Info($"starting to run job {job}");
            return jobRunner.Run(job);
        }

        internal override void JobSucceeded(JobData job)
        {
            StdErr.Info($"job {job} finished normally");
            jobPersister.Succeeded(job);
`        }

        protected override void JobFailed(JobData job, Exception ex)
        {
            job = jobPersister.Failed(job);
            if (job.CanRetry)
                StdErr.Info($"job {job} failed but will be retried, failed with: '{ex.Message}'");
            else
                StdErr.Error($"job {job} failed: '{ex}'");
        }

        protected override void WaitForAllRunningJobsToFinish()
        {
            StdErr.Info($"waiting all jobs to finish");
            jobClaimer.WaitForRunningJobsToFinish();
        }

        protected override void CleanUpPreviouslyClaimedJobs()
        {
            StdErr.Info($"Attempting clean up of previously running jobs");
            jobPersister.CleanUp();
        }
    }

    class JobData
    {
        public bool CanRetry { get; internal set; }
        public Guid Jobid { get; }
        public override string ToString() => $"{Jobid}"; //TODO: type of job
    }

    public interface IJob
    {
        //Stream Run(Stream input, TextWriter log);
        Task Run();
    }
}
