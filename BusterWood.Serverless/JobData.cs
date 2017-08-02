using BusterWood.CommandLine;
using System;
using System.IO;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;

namespace BusterWood.Serverless
{

    public class JobData
    {
        static readonly string[] none = new String[0];
        public Guid Jobid { get; set; }
        public int Attempts { get; set; } = 0;
        public int MaxRetries { get; set; } = 3;
        public bool CanRetry => Attempts < MaxRetries;
        public string FullAssemblyName { get; set; }
        public string[] Args { get; set; } = none;
        public Stream Input { get; set; }
        public Stream Output { get; set; }
        public Stream Logging { get; set; }
        public object ExitCode { get; set; }

        public override string ToString() => $"{Jobid}"; //TODO: type of job
    }
}
