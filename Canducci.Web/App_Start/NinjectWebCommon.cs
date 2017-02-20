[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(Canducci.Web.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethod(typeof(Canducci.Web.App_Start.NinjectWebCommon), "Stop")]

namespace Canducci.Web.App_Start
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;
    using Models;
    using DocumentDB;    
    using System.Configuration;

    public static class NinjectWebCommon 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();        
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }      
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                RegisterServices(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }       
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Bind<ConnectionDocumentDB>()                
                .ToSelf()
                .WithConstructorArgument("url", ConfigurationManager.AppSettings["url"])
                .WithConstructorArgument("key", ConfigurationManager.AppSettings["key"])
                .WithConstructorArgument("database", ConfigurationManager.AppSettings["database"]);

            kernel.Bind<RepositoryCarAbstract>().To<RepositoryCar>();
            kernel.Bind<RepositoryCreditAbstract>().To<RepositoryCredit>();
        }          
              
    }
}
