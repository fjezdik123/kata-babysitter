using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ninject;

namespace PayoutCalcApp.Infrastructure
{
    public class IocContainer
    {
        public static IKernel Kernel { get; private set; }

        public static IKernel BootstrapKernel()
        {
            Kernel = new StandardKernel();
            return Kernel;
        }

        public static void RegisterBindings()
        {
            Kernel.Bind<ICacheService>().To<CacheManager>().InSingletonScope();
        }

        public static T Resolve<T>() => Kernel.Get<T>();
        public static bool CanResolve<T>() => Kernel.CanResolve<T>();
    }
}