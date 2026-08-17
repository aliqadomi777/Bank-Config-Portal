using System.Collections.Generic;
using WebPortal.Application.DTO.Allocations;

namespace WebPortal.Application.Interfaces
{
    public interface IAllocationService
    {
        IEnumerable<AllocationResponseDto> GetAllAllocations(int counterId, int bankId);
        AllocationResponseDto GetAllocationById(int allocationId, int bankId);
        int CreateAllocation(AllocationCreateRequestDto request, int bankId);
        bool UpdateAllocation(AllocationUpdateRequestDto request, int bankId);
        bool DeleteAllocation(int allocationId, int bankId);
    }
}
