using BusterWood.CommandLine;
using BusterWood.Serverless;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestHost
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread.Sleep(2000);
            var runner = new AppDomainJobRunner();
            var jobs = Enumerable.Range(0, 1).Select(i => RunJobAsync(runner)).ToArray();

            Task.WaitAll(jobs);

            //job.Output.CopyTo(Console.OpenStandardOutput());
        }

        private static async Task RunJobAsync(AppDomainJobRunner runner)
        {
            var job = new JobData
            {
                Jobid = Guid.NewGuid(),
                FullAssemblyName = "HelloWorld, version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
                Input = new MemoryStream(),
                Output = new MemoryStream(),
                Logging = new MemoryStream(),
            };

            await runner.Run(job);

            job.Output.Seek(0, SeekOrigin.Begin);
            var output = new StreamReader(job.Output, Encoding.UTF8, false, 4096, true);

            Console.WriteLine(output.ReadToEnd());
        }
    }
}
