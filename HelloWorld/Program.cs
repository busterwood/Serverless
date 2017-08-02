using BusterWood.CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloWorld
{
    class Program
    {
        static void Main(string[] args)
        {
            var argList = Std.Initialise(args);
            Std.LogInfo("starting");
            Std.Out.WriteLine("hello world");
            Std.Close();
        }
    }
}
