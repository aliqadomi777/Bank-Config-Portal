using System.Collections.Generic;
using WebPortal.Application.DTO.Allocations;

namespace WebPortal.Application.Interfaces
{
    public interface IAllocationService
    {
        IEnumerable<AllocationResponseDto> GetAllAllocations(int counterId);
        AllocationResponseDto GetAllocationById(int allocationId);
        int CreateAllocation(AllocationCreateRequestDto request);
        bool UpdateAllocation(AllocationUpdateRequestDto request);
        bool DeleteAllocation(int allocationId);
    }
}
