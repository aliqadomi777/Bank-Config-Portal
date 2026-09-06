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
                   .As<IFetchableByBankUserRepository<UserModel>>()
                   .InstancePerDependency();
            builder.RegisterType<BranchRepository>()
                   .WithParameter("connectionString", _connectionString)
                   .AsSelf()
                   .As<IFetchableRepository<BranchModel>>()
                   .As<IListableRepository<BranchModel>>()
                   .As<IAddableRepository<BranchModel>>()
                   .As<IUpdateableRepository<BranchModel>>()
                   .As<IDeleteableRepository<BranchModel>>()
                   .InstancePerDependency();
            builder.RegisterType<ServiceRepository>()
                   .WithParameter("connectionString", _connectionString)
                   .AsSelf()
                   .As<IFetchableRepository<ServiceModel>>()
                   .As<IListableRepository<ServiceModel>>()
                   .As<IAddableRepository<ServiceModel>>()
                   .As<IUpdateableRepository<ServiceModel>>()
                   .As<IDeleteableRepository<ServiceModel>>()
                   .InstancePerDependency();

            builder.RegisterType<CounterRepository>()
                   .WithParameter("connectionString", _connectionString)
                   .AsSelf()
                   .As<IFetchableRepository<CounterModel>>()
                   .As<IListableRepository<CounterModel>>()
                   .As<IAddableRepository<CounterModel>>()
                   .As<IUpdateableRepository<CounterModel>>()
                   .As<IDeleteableRepository<CounterModel>>()
                   .InstancePerDependency();

            builder.RegisterType<AllocationRepository>()
                   .WithParameter("connectionString", _connectionString)
                   .AsSelf()
                   .As<IFetchableRepository<AllocationModel>>()
                   .As<IListableRepository<AllocationModel>>()
                   .As<IAddableRepository<AllocationModel>>()
                   .As<IUpdateableRepository<AllocationModel>>()
                   .As<IDeleteableRepository<AllocationModel>>()
                   .As<IAllocationRepository>()
                   .InstancePerDependency();

            builder.RegisterType<CounterTypeRepository>()
                   .WithParameter("connectionString", _connectionString)
                   .AsSelf()
                   .As<IGetAllRepository<CounterTypeModel>>()
                   .InstancePerDependency();

            builder.RegisterType<TicketingScreenRepository>()
                   .WithParameter("connectionString", _connectionString)
                   .As<ITicketingScreenRepository>()
                   .InstancePerDependency();
        }
    }
}
