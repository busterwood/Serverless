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

            var pipeEndsIn = InProcessPipe.CreatePipe();
            var pipeEndsOut = InProcessPipe.CreatePipe();
            var pipeEndsLog = InProcessPipe.CreatePipe();

            using (var pipeIn = pipeEndsIn.OutStream()) 
            using (var pipeOut = pipeEndsOut.InStream())
            using (var pipeLog = pipeEndsLog.InStream())
            {
                var args = new List<string> {
                    Std.PipeInArg, pipeEndsIn.In.ToString(),
                    Std.PipeOutArg, pipeEndsOut.Out.ToString(),
                    Std.PipeLogArg, pipeEndsLog.Out.ToString()
                };
                args.AddRange(job.Args);
                Std.LogInfo($"Starting {job} in new app domain");
                Task<int> jobTask = Task.Factory.StartNew<int>(() => domain.ExecuteAssemblyByName(job.FullAssemblyName, args.ToArray()), CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);

                var writeInput = job.Input.CopyToAsync(pipeIn);
                var readOutput = pipeOut.CopyToAsync(job.Output);
                var readLogging = pipeLog.CopyToAsync(job.Logging);

                job.ExitCode = await jobTask;

                Std.LogInfo($"{job} finished with exit code {job.ExitCode}, Unloading AppDomain");
                AppDomain.Unload(domain);

                await Task.WhenAll(readOutput, readLogging); // TODO: wait for input?
            }
            var elapsed = DateTime.UtcNow - startTime;
            Std.LogInfo($"Job {job} took {elapsed.TotalMilliseconds:N0}MS");
        }
        
    }
}