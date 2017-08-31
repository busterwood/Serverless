using System;
using System.Threading;
using System.Threading.Tasks;
using BusterWood.Mapper;
using BusterWood.CommandLine;

namespace BusterWood.Serverless
{
    /// <summary>
    /// Used by this instance to wait for work and shutdown.
    /// </summary>
    abstract class JobScheduler
    {
        /// <summary>Wait for new work to become available for this host</summary>
        public abstract Task<JobStart> WaitForWork(CancellationToken cancel);

        /// <summary>Wait for this host to finish all its running jobs.  If the timeout is reached then job will attempt to be terminated</summary>
        /// 
        public abstract void WaitForRunningJobsToFinish(TimeSpan timeout);
    }

    class StoreAndNotifyJobScheduler : JobScheduler
    {
        JobPersister jobPersister;
        JobNotifier jobNotifier;

        public override async Task<JobStart> WaitForWork(CancellationToken cancel)
        {
            for (;;)
            {
                var result = await jobPersister.TryClaimJob(cancel);
                if (result.Job != null)
                    return result.Job;
                if (cancel.IsCancellationRequested)
                    return null;
                await jobNotifier.WaitForWork(result.LastSeenTimestamp);
            }
        }

        public override void WaitForRunningJobsToFinish(TimeSpan timeout)
        {
            var lastRunning = -1;
            for(;;)
            {
                int running = jobPersister.RunningJobCount();
                if (running == 0)
                    return;
                if (running != lastRunning)
                {
                    Std.LogVerbose($"Waiting for {running} running jobs to finish");
                    lastRunning = running;
                }
                Thread.Sleep(500);
            }
        }
    }
}
