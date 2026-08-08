using Autofac;
using WebPortal.Application.Interfaces;
using WebPortal.Application.Services;
namespace WebPortal.Infrastructure
{
    public class ApplicationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UserService>()
                .As<IUserService>()
                .InstancePerDependency();
            builder.RegisterType<ServiceManager>()
                .As<IServiceManager>()
                .InstancePerDependency();
            builder.RegisterType<BranchService>()
                .As<IBranchService>()
                .InstancePerDependency();
        }
    }
}
