using System;
using System.Collections.Generic;
using System.IO;

namespace BusterWood.Serverless
{

    public class JobData
    {
        static readonly string[] none = new String[0];
        public Guid JobId { get; set; }
        public int Attempts { get; set; } = 0;
        public int MaxRetries { get; set; } = 3;
        public bool CanRetry => Attempts < MaxRetries;
        public string FullPath { get; set; }
        public string[] Args { get; set; } = none;
        public IDictionary<string, string> Environment { get; set; } = new Dictionary<string, string>();
        public Stream Input { get; set; }
        public Stream Output { get; set; }
        public Stream Logging { get; set; }
        public object ExitCode { get; set; }

        public override string ToString() => $"{JobId}"; //TODO: type of job

        public IReadOnlyList<JobStart> Starts { get; set; }
        public IReadOnlyList<JobEnd> Ends { get; set; }

    }
    
    public class Job
    {
        public Guid JobId { get; set; }
        public JobType JobType { get; set; }
        public int MaxAttempts { get; set; }
        public DateTimeOffset JobAdded { get; set; }
        public Stream Input { get; set; }
    }

    public class JobType
    {
        public int JobTypeId { get; set; }
        public int JobTypeSeq { get; set; }
        public int JobTypeName { get; set; }
        public int MaxAttempts { get; set; }
        public string InitialFolder { get; set; }
        public string ExePath { get; set; }
        public DateTimeOffset JobTypeAdded { get; set; }
        public IReadOnlyDictionary<string, string> Environment { get; set; } = new Dictionary<string, string>();
    }



    /// <summary>
    /// Record of the start of a job on a host
    /// </summary>
    public class JobStart
    {
        public Guid JobId { get; set; }
        public int Attempt { get; set; }
        public DateTimeOffset StartedAt { get; set; }
        public string HostName { get; set; }
    }

    /// <summary>
    /// Record of the end of a job
    /// </summary>
    public class JobEnd
    {
        public Guid JobId { get; set; }
        public int Attempt { get; set; }
        public DateTimeOffset EndAt { get; set; }
        public int ExitCode { get; set; } = -1;
        public Stream Output { get; set; }
        public Stream Logging { get; set; }
        public string Exception { get; set; }

        //public JobStart Start { get; set; }

        public bool Succeeded => ExitCode == 0;
    }
}
