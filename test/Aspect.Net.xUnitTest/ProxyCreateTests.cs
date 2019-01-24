using System;
using System.Collections.Generic;
using System.Text;
using Aspect.Net.TestModel;
using Xunit;

namespace Aspect.Net.xUnitTest
{
    public class ProxyCreateTests
    {
        [Fact]
        public void EmptyCreate_ClassA_ClassA()
        {
            var proxy = new Proxy();

            var a = proxy.Create<ClassA>();

            Assert.True(a.GetType() == typeof(ClassA));
        }

        [Fact]
        public void DefaultCreate_ClassA_NotClassA()
        {
            var proxy = new Proxy(new DefaultAspect());

            var a = proxy.Create<ClassA>();

            Assert.True(a.GetType() != typeof(ClassA));
        }
    }
}
