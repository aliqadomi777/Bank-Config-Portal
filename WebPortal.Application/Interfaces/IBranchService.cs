using System.Collections.Generic;
using WebPortal.Application.DTO.Branch;

namespace WebPortal.Application.Interfaces
{
    public interface IBranchService
    {
        IEnumerable<BranchResponseDto> GetAllBranches(int bankId);
        BranchResponseDto GetBranchById(int branchId);
        int CreateBranch(BranchCreateRequestDto request);
        bool UpdateBranch(BranchUpdateRequestDto request);
        bool DeleteBranch(int branchId);
    }
}
