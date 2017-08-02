using BusterWood.CommandLine;

namespace HelloWorld
{
    class Program
    {
        static void Main(string[] args)
        {
            var argList = Std.Initialise(args);
            Std.LogInfo("starting");
            Std.Out.WriteLine("hello world");
        }
    }
}
