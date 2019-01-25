using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Aspect.Net.TestModel;
using Xunit;

namespace Aspect.Net.xUnitTest
{
    public class ProxyCallTests
    {
        [Fact]
        public void EmptyCreate_ClassA_Call_False()
        {
            var proxy = new Proxy();

            var a = proxy.Create<ClassA>();

            Assert.False(a.Call());
        }

        [Fact]
        public void CustomCreate_ClassA_Call_True()
        {
            var proxy = new Proxy(new DefaultAspect(ctx =>
            {
                ctx.ReturnValue = true;
                return Task.CompletedTask;
            }));

            var a = proxy.Create<ClassA>();

            Assert.True(a.Call());
        }

        [Fact]
        public void DefaultCreate_ClassA_Call_False()
        {
            var proxy = new Proxy(new DefaultAspect());

            var a = proxy.Create<ClassA>();

            Assert.False(a.Call());
        }

        [Fact]
        public async Task DefaultCreate_ClassA_CallAsync()
        {
            var proxy = new Proxy(new DefaultAspect());
            var a = proxy.Create<ClassA>();

            var time = DateTime.Now;
            await a.CallAsync();
            var act = (DateTime.Now - time).TotalMilliseconds;

            Assert.True(act > 2000);
        }

        [Fact]
        public async Task DefaultCreate_ClassA_CallAsync_False()
        {
            var proxy = new Proxy(new DefaultAspect());
            var a = proxy.Create<ClassA>();

            var act = await a.CallBoolReturnAsync();
                
            Assert.False(act);
        }

        [Fact]
        public async Task CustomCreate_ClassA_CallAsync_True()
        {
            var proxy = new Proxy(new DefaultAspect(async ctx =>
            {
                await ctx.InvokeAsync();
                ctx.ReturnValue = true;
            }));
            var a = proxy.Create<ClassA>();

            var act = await a.CallBoolReturnAsync();

            Assert.True(act);
        }
    }
}
