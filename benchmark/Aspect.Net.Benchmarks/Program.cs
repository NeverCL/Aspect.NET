using System;
using BenchmarkDotNet.Running;

namespace Aspect.Net.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<CallVsProxyCall>();
        }
    }
}
