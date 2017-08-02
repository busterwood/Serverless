using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Threading.Tasks;
using BusterWood.CommandLine;
using System.Threading;
using System.IO;

namespace BusterWood.Serverless
{
    public abstract class JobRunner
    {
        public abstract Task Run(JobData job);
    }

    /// <summary>
    /// Runs the console app for the job in a new app domain
    /// </summary>
    public class AppDomainJobRunner : JobRunner
    {
        public override async Task Run(JobData job)
        {
            var startTime = DateTime.UtcNow;
            var domain = AppDomain.CreateDomain($"Job {job}");
            try
            {
                await RunJobInDomain(job, domain);
            }
            finally
            {
                Std.LogInfo($"Unloading AppDomain {domain.FriendlyName}");
                AppDomain.Unload(domain);
            }
            var elapsed = DateTime.UtcNow - startTime;
            Std.LogInfo($"Job {job} took {elapsed.TotalMilliseconds:N0}MS");
        }

        private static async Task RunJobInDomain(JobData job, AppDomain domain)
        {
            using (var pipeIn = new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.Inheritable))
            using (var pipeOut = new AnonymousPipeServerStream(PipeDirection.In, HandleInheritability.Inheritable))
            using (var pipeLog = new AnonymousPipeServerStream(PipeDirection.In, HandleInheritability.Inheritable))
            {
                var args = new List<string> {
                    Std.PipeInArg, pipeIn.GetClientHandleAsString(),
                    Std.PipeOutArg, pipeOut.GetClientHandleAsString(),
                    Std.PipeLogArg, pipeLog.GetClientHandleAsString()
                };
                args.AddRange(job.Args);
                Std.LogInfo($"Starting {job} in new app domain");
                Task<int> jobTask = Task.Factory.StartNew<int>(() => domain.ExecuteAssemblyByName(job.FullAssemblyName, args.ToArray()), CancellationToken.None, TaskCreationOptions.PreferFairness, TaskScheduler.Default);

                var writeInput = job.Input.CopyToAsync(pipeIn);
                var readOutput = pipeOut.CopyToAsync(job.Output);
                var readLogging = pipeLog.CopyToAsync(job.Logging);

                job.ExitCode = await jobTask;

                // only close client handle AFTER the program is complete as it is running in the same process (but different AppDomain)
                pipeIn.DisposeLocalCopyOfClientHandle();
                pipeOut.DisposeLocalCopyOfClientHandle();
                pipeLog.DisposeLocalCopyOfClientHandle();

                Std.LogInfo($"{job} finished with exit code {job.ExitCode}");
                await Task.WhenAll(readOutput, readLogging); // TODO: wait for input?
            }
        }
    }
}