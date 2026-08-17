using WebPortal.Domain.Model;

namespace WebPortal.Application.Interfaces
{
    public interface IBankAuthorizationService
    {
        ServiceModel GetServiceForBank(
            int serviceId,
            int bankId);

        BranchModel GetBranchForBank(
            int branchId,
            int bankId);

        CounterModel GetCounterForBank(
            int counterId,
            int bankId);

        AllocationModel GetAllocationForBank(
            int allocationId,
            int bankId);
    }
}