using System.ComponentModel.DataAnnotations;

namespace WebPortal.Application.DTO.Branch
{
    public class BranchBaseRequestDto
    {
        [Required(ErrorMessage = "Branch name is required.")]
        [MaxLength(100, ErrorMessage = "Branch name can't exceed 100 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9.\-_ /]+$", ErrorMessage = "Branch name contains invalid characters.")]
        public string BranchNameEN { get; set; }

        [Required(ErrorMessage = "Branch name is required.")]
        [MaxLength(100, ErrorMessage = "Branch name can't exceed 100 characters.")]
        [RegularExpression(@"^[\u0600-\u06FF0-9.\-_ /]+$", ErrorMessage = "Branch name contains invalid characters.")]
        public string BranchNameAR { get; set; }

        [Required(ErrorMessage = "Branch status is required.")]
        public bool BranchStatus { get; set; }

    }
}
