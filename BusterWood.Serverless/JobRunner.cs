using System;
using System.Threading.Tasks;

namespace BusterWood.Serverless
{
    abstract class JobRunner
    {
        public abstract Task Run(JobData job);
    }
}