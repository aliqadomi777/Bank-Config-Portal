using System.ComponentModel.DataAnnotations;

namespace WebPortal.Application.DTO.Branch
{
    public class BranchCreateRequestDto : BranchBaseRequestDto
    {
        [Required(ErrorMessage = "Bank ID is required.")]

        public int BankId { get; set; }

    }
}
