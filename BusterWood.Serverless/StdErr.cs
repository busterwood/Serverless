using System;

namespace BusterWood.Serverless
{

    static class StdErr
    {
        static readonly string Program = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;

        public static void Debug(string msg)
        {
            Console.Error.WriteLine($"{Program}: DEBUG: {msg}");
        }

        public static void Info(string msg)
        {
            Console.Error.WriteLine($"{Program}: INFO: {msg}");
        }

        public static void Error(string msg)
        {
            Console.Error.WriteLine($"{Program}: ERROR: {msg}");
        }
    }
}