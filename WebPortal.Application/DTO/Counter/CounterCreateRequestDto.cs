using System.ComponentModel.DataAnnotations;

namespace WebPortal.Application.DTO.Counter
{
    public class CounterCreateRequestDto : CounterBaseRequestDto
    {
        [Required(ErrorMessage = "Branch ID is required.")]
        public int BranchId { get; set; }

    }
}
