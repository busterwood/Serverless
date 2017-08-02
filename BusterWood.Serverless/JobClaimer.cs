using System;
using System.Threading;
using System.Threading.Tasks;
using BusterWood.Mapper;
using BusterWood.CommandLine;

namespace BusterWood.Serverless
{
    abstract class JobClaimer
    {
        public abstract Task<JobData> ClaimNextJob(CancellationToken cancel);

        public abstract void WaitForRunningJobsToFinish();
    }

    class StoreAndNotifyJobClaimer : JobClaimer
    {
        JobPersister jobPersister;
        JobNotifier jobNotifier;

        public override async Task<JobData> ClaimNextJob(CancellationToken cancel)
        {
            for (;;)
            {
                var job = await jobPersister.TryClaimJob(cancel);
                if (job != null || cancel.IsCancellationRequested)
                    return job;
                await jobNotifier.NewJob();
            }
        }

        public override void WaitForRunningJobsToFinish()
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