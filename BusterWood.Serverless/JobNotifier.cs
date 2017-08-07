using System;
using System.Threading.Tasks;

namespace BusterWood.Serverless
{
    abstract class JobNotifier
    {
        public abstract Task WaitForWork(long lastSeenTimestamp);
    }
}