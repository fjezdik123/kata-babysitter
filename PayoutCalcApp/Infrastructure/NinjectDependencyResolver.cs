using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ninject;

namespace PayoutCalcApp.Infrastructure
{
    public class NinjectDependencyResolver : IDependencyResolver
    {
        private readonly IKernel _kernel;

        public NinjectDependencyResolver()
        {
            _kernel = IocContainer.BootstrapKernel();
        }

        public object GetService(Type serviceType) => _kernel.TryGet(serviceType);
        public IEnumerable<object> GetServices(Type serviceType) => _kernel.GetAll(serviceType);
    }
}