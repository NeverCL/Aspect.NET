using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Aspect.Net.TestModel
{
    public class ClassA
    {
        public virtual void CallNoArg()
        {
            Console.WriteLine("CallNoArg");
        }
        public virtual bool Call()
        {
            return false;
        }

        public virtual async Task CallAsync()
        {
            await Task.Delay(2000);
        }

        public virtual async Task<bool> CallBoolReturnAsync()
        {
            await Task.Delay(2000);
            return false;
        }

        public virtual async Task<string> CallStrReturnAsync()
        {
            await Task.Delay(1);
            return "hi";
        }

        public virtual string CallStrArgStrReturn(string str)
        {
            return str;
        }
        public virtual async Task<bool> CallBoolArgBoolReturnAsync(bool b)
        {
            await Task.Delay(1);
            return b;
        }
    }
}
