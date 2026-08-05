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
        }
    }
}
