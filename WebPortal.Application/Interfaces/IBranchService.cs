using System.Collections.Generic;
using WebPortal.Application.DTO.Branch;

namespace WebPortal.Application.Interfaces
{
    public interface IBranchService
    {
        IEnumerable<BranchResponseDto> GetAllBranches(int bankId);
        BranchResponseDto GetBranchById(int branchId, int bankId);
        int CreateBranch(BranchCreateRequestDto request, int bankId);
        bool UpdateBranch(BranchUpdateRequestDto request, int bankId);
        bool DeleteBranch(int branchId, int bankId);

    }
}
