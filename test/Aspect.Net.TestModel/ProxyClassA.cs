using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Aspect.Net.TestModel
{
    public class ProxyClassA
    {
        private readonly IAspect _aspect;

        public ProxyClassA(IAspect aspect)
        {
            this._aspect = aspect;
        }

        #region Normal

        public void Call()
        {
            _aspect.InvokeAsync<object>(GetContext());
        }

        public bool CallNoArgReturnBool()
        {
            return _aspect.InvokeAsync<bool>(GetContext()).Result;
        }

        public ProxyClassA CallNoArgReturnObj()
        {
            return _aspect.InvokeAsync<ProxyClassA>(GetContext()).Result;
        }

        #endregion

        #region Task
        public Task CallAsync()
        {
            return _aspect.InvokeAsync<Task>(GetContext());
        }

        public Task<bool> CallNoArgReturnBoolAsync()
        {
            return _aspect.InvokeAsync<bool>(GetContext());
        }

        public Task<ProxyClassA> CallNoArgReturnObjAsync()
        {
            return _aspect.InvokeAsync<ProxyClassA>(GetContext());
        }

        #endregion

        private AspectContext GetContext()
        {
            return new AspectContext(this, (MethodInfo)MethodBase.GetCurrentMethod(), null, null);
        }
    }
}
