using System.ComponentModel.DataAnnotations;

namespace WebPortal.Application.DTO.Allocations
{
    public class AllocationCreateRequestDto : AllocationBaseRequestDto
    {
        [Required(ErrorMessage = "Counter ID is required.")]
        public int CounterId { get; set; }
    }
}
