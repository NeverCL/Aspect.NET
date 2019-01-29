using System;
using System.Collections.Generic;
using System.Text;
using Aspect.Net.TestModel;
using BenchmarkDotNet.Attributes;

namespace Aspect.Net.Benchmarks
{
    [CoreJob, ClrJob]
    public class CallVsProxyCall
    {
        private readonly ClassA _proxy;
        private readonly ClassA _instance;

        public CallVsProxyCall()
        {
            _proxy = new Proxy(async ctx => await ctx.InvokeAsync()).Create<ClassA>();
            _proxy.Call();
            _instance = new ClassA();
            _instance.Call();
        }

        [Benchmark]
        public void ProxyCall() => _proxy.Call();

        [Benchmark(Baseline = true)]
        public void Call() => _instance.Call();
    }
}
