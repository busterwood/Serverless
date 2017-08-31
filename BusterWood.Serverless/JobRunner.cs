using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Threading.Tasks;
using BusterWood.CommandLine;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Linq;

namespace BusterWood.Serverless
{
    public abstract class JobRunner
    {
        public virtual async Task Run(JobData job)
        {
            JobStart start = await JobStarted(job);
            JobEnd end;
            try
            {
                end = await RunCore(job, start);
            }
            catch (Exception ex)
            {git 
                await JobFailed(job, start, ex);
                return;
            }
            await JobFinished(start, end);
        }

        protected virtual Task<JobStart> JobStarted(JobData job)
        {
            var start = new JobStart
            {
                JobId = job.JobId,
                Attempt = job.Attempts, // this should be correctly 
                HostName = Environment.MachineName,
                StartedAt = DateTimeOffset.Now,
            };
            return StoreStart(start);
        }

        protected abstract Task<JobStart> StoreStart(JobStart start);

        protected abstract Task<JobEnd> RunCore(JobData job, JobStart jobStart);

        protected virtual Task JobFailed(JobData job, JobStart jobStart, Exception ex)
        {
            var failure = new JobEnd
            {
                JobId = jobStart.JobId,
                Attempt = jobStart.Attempt,
                EndAt = DateTime.UtcNow,
                Exception = ex.ToString(),
            };
            return StoreEnd(failure);
        }

        protected abstract Task StoreEnd(JobEnd failure);

        protected virtual Task JobFinished(JobStart start, JobEnd jobEnd)
        {
            return StoreEnd(jobEnd);
        }
    }

    /// <summary>
    /// Runs the console app for the job in a new app domain
    /// </summary>
    public class AppDomainJobRunner : JobRunner
    {
        protected override Task JobStarted(JobData job)
        {
            throw new NotImplementedException();
        }

        protected override async Task RunCore(JobData job)
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
                Task<int> jobTask = Task.Factory.StartNew<int>(() => domain.ExecuteAssemblyByName(job.FullPath, args.ToArray()), CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);

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

        protected override Task RecordJobFailure(JobData job, Exception ex)
        {
            throw new NotImplementedException();
        }

        protected override Task RecordJobFinished(JobData job)
        {
            throw new NotImplementedException();
        }
    }

    public class NewProcessJobRunner : JobRunner
    {
        public override async Task Run(JobData job)
        {
            var startTime = DateTime.UtcNow;
            var info = new ProcessStartInfo()
            {
                Arguments = string.Join(" ", job.Args.Select(a => "\"" + a + "\"")),
                WorkingDirectory = Path.GetDirectoryName(job.FullPath),
                FileName = Path.GetFileName(job.FullPath),
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = !Debugger.IsAttached,
            };
            info.Environment.AddRange(job.Environment);
            var proc = Process.Start(info);

            var writeInput = job.Input.CopyToAsync(proc.StandardInput.BaseStream);
            var readOutput = proc.StandardOutput.BaseStream.CopyToAsync(job.Output);
            var readLogging = proc.StandardError.BaseStream.CopyToAsync(job.Logging);

            await Task.Run(() => proc.WaitForExit());
            job.ExitCode = proc.ExitCode;

            Std.LogInfo($"{job} finished with exit code {job.ExitCode}");

            await Task.WhenAll(readOutput, readLogging); 

            var elapsed = DateTime.UtcNow - startTime;
            Std.LogInfo($"Job {job} took {elapsed.TotalMilliseconds:N0}MS");
        }
    }
}