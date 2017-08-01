using System;
using System.Threading;
using System.Threading.Tasks;

namespace BusterWood.Serverless
{
    abstract class JobPersister
    {
        public abstract void Succeeded(JobData job);

        public abstract JobData Failed(JobData job);

        public abstract Task<JobData> TryClaimJob(CancellationToken cancel);

        public abstract int RunningJobCount();

        internal void CleanUp()
        {
            throw new NotImplementedException();
        }
    }
}