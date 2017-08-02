using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BusterWood.CommandLine
{
    public static class ListExtensions
    {
        public static string StringArg(this IList<string> list, string argName, string @default = null)
        {
            var result = @default;
            var idx = list.IndexOf(argName);
            if (idx >= 0)
            {
                list.RemoveAt(idx);
                if (idx < list.Count)
                {
                    result = list[idx];
                    list.RemoveAt(idx);
                }
            }
            return result;
        }
    }

    public static class Std
    {
        public const string PipeInArg = "--pipe-in";
        public const string PipeOutArg = "--pipe-out";
        public const string PipeLogArg = "--pipe-log";

        static readonly string Program = Assembly.GetEntryAssembly().GetName().Name;

        //public static Stream Input { get; private set; }
        //public static Stream Output { get; private set; }
        //public static Stream Logging { get; private set; }

        public static TextReader In { get; private set; }
        public static TextWriter Out { get; private set; }
        public static TextWriter Log { get; private set; }

        static Std()
        {
            In = Console.In;
            Out = Console.Out;
            Log = Console.Error;
        }

        public static List<string> Initialise(string[] args)
        {
            var argList = args.ToList();

            var pipeIn = argList.StringArg(PipeInArg);
            if (!string.IsNullOrWhiteSpace(pipeIn))
                In = new StreamReader(new AnonymousPipeClientStream(PipeDirection.In, pipeIn));

            var pipeOut = argList.StringArg(PipeOutArg);
            if (!string.IsNullOrWhiteSpace(pipeOut))
                Out = new StreamWriter(new AnonymousPipeClientStream(PipeDirection.Out, pipeOut));

            var pipeLog = argList.StringArg(PipeLogArg);
            if (!string.IsNullOrWhiteSpace(pipeLog))
                Log = new StreamWriter(new AnonymousPipeClientStream(PipeDirection.Out, pipeOut));

            return argList;
        }

        public static void LogInfo(string message)
        {
            LogLine(message, "INFO");
        }

        public static void LogError(string message)
        {
            LogLine(message, "ERROR");
        }

        public static void LogWarning(string message)
        {
            LogLine(message, "WARNING");
        }

        public static void LogVerbose(string message)
        {
            LogLine(message, "VERBOSE");
        }

        private static void LogLine(string message, string level)
        {
            Log.WriteLine($"{Program}: {level}: {DateTime.UtcNow:u}: {message}");
        }
    }
}
