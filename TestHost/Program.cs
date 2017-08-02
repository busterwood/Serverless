using BusterWood.CommandLine;
using BusterWood.Serverless;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestHost
{
    class Program
    {
        static void Main(string[] args)
        {

            var runner = new AppDomainJobRunner();
            for(int i = 0; i < 10; i++)
            {
                var job = new JobData
                {
                    Jobid = Guid.NewGuid(),
                    FullAssemblyName = "HelloWorld, version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
                    Input = new MemoryStream(),
                    Output = new MemoryStream(),
                    Logging = new MemoryStream(),
                };

                var task = runner.Run(job);
                task.Wait();
                job.Output.Seek(0, SeekOrigin.Begin);
                job.Output.CopyTo(Console.OpenStandardOutput());

            }

            //job.Output.CopyTo(Console.OpenStandardOutput());
        }
    }
}
