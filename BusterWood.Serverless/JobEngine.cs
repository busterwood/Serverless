using BusterWood.CommandLine;
using System;
using System.Collections;
using System.IO;
using System.Security.Policy;
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
                var job = await RunnableJob(cancel);
                if (job == null)
                    break; // we have been cancelled
                var dontWait = TryToRunJob(job);
            }            
        }

        /// <summary>
        /// Claim the next job that can be run by this engine
        /// </summary>
        internal abstract Task<JobData> RunnableJob(CancellationToken cancel);

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
        readonly TimeSpan shutdownTimeout;
        readonly JobScheduler jobClaimer;
        readonly JobRunner jobRunner;
        readonly JobPersister jobPersister;

        public JobEngine(JobScheduler jobClaimer, JobRunner jobRunner, JobPersister jobPersister, IDictionary environment = null)
        {
            this.jobClaimer = jobClaimer;
            this.jobRunner = jobRunner;
            this.jobPersister = jobPersister;
            var env = (environment ?? Environment.GetEnvironmentVariables());
            if (!TimeSpan.TryParse(env["JOB_HOST_SHUTDOWN_TIMEOUT"]?.ToString() ?? "", out shutdownTimeout))
                shutdownTimeout = TimeSpan.FromSeconds(30);
        }

        internal override async Task<JobData> RunnableJob(CancellationToken cancel)
        {
            Std.LogInfo($"waiting for next job");
            var job = await jobClaimer.WaitForWork(cancel);
            if (job != null)
                Std.LogInfo($"claimed job {job}");
            return job;
        }

        protected override Task RunJob(JobData job)
        {
            Std.LogInfo($"starting to run job {job}");
            return jobRunner.Run(job);
        }

        internal override void JobSucceeded(JobData job)
        {
            Std.LogInfo($"job {job} finished normally");
            jobPersister.Succeeded(job);
        }

        protected override void JobFailed(JobData job, Exception ex)
        {
            job = jobPersister.Failed(job);
            if (job.CanRetry)
                Std.LogInfo($"job {job} failed but will be retried, failed with: '{ex.Message}'");
            else
                Std.LogError($"job {job} failed: '{ex}'");
        }

        protected override void WaitForAllRunningJobsToFinish()
        {
            Std.LogInfo($"waiting all jobs to finish");
            jobClaimer.WaitForRunningJobsToFinish(shutdownTimeout);
        }

        protected override void CleanUpPreviouslyClaimedJobs()
        {
            Std.LogInfo($"Attempting clean up of previously running jobs");
            jobPersister.CleanUp();
        }
    }

    public interface IJob
    {
        //Stream Run(Stream input, TextWriter log);
        Task Run();
    }

    // TODO: move environment access helper to this class
    //public static class Env
    //{
    //    public static TimeSpan TimeSpan(string )
    //}
}
