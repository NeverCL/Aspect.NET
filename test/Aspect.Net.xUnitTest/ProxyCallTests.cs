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
        public void DefaultCreate_ClassA_Call_True()
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
    }
}
