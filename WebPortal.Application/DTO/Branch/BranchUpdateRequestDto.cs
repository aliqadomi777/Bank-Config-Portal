using System.ComponentModel.DataAnnotations;

namespace WebPortal.Application.DTO.Branch
{
    public class BranchUpdateRequestDto : BranchBaseRequestDto
    {
        [Required(ErrorMessage = "Branch ID is required.")]

        public int BranchId { get; set; }

    }
}
