using Autofac;
using WebPortal.Domain.Interfaces;
using WebPortal.Domain.Model;
using WebPortal.Infrastructure.Repositories;

namespace WebPortal.Infrastructure
{
    public class InfrastructureModule : Module
    {
        private readonly string _connectionString;

        public InfrastructureModule(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UserRepository>()
                   .WithParameter("connectionString", _connectionString)
                   .AsSelf()
                   .As<IFetchableRepository<UserModel>>()
                   .InstancePerDependency();
        }
    }
}
