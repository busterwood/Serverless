using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace BusterWood.CommandLine
{
    static class InProcessPipe
    {
        [DllImport("kernel32.dll")]
        static extern bool CreatePipe(out IntPtr hReadPipe, out IntPtr hWritePipe, IntPtr lpPipeAttributes, uint nSize);

        public static PipeEnds CreatePipe()
        {
            IntPtr hread, hwrite;
            CreatePipe(out hread, out hwrite, IntPtr.Zero, 0);
            return new PipeEnds(hread, hwrite);
        }

        public static Stream InStream(string handle) => InStream((IntPtr)int.Parse(handle));
        public static Stream InStream(IntPtr handle) => new FileStream(handle, FileAccess.Read);
        public static Stream OutStream(string handle) => OutStream((IntPtr)int.Parse(handle));
        public static Stream OutStream(IntPtr handle) => new FileStream(handle, FileAccess.Write);
    }

    struct PipeEnds
    {
        public IntPtr In { get; }
        public IntPtr Out { get; }

        public PipeEnds(IntPtr @in, IntPtr @out)
        {
            In = @in;
            Out = @out;
        }

        public Stream InStream() => InProcessPipe.InStream(In);
        public Stream OutStream() => InProcessPipe.OutStream(Out);
    }
}
