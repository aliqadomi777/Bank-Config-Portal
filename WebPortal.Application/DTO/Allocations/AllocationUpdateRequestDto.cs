using System.ComponentModel.DataAnnotations;

namespace WebPortal.Application.DTO.Allocations
{
    public class AllocationUpdateRequestDto : AllocationBaseRequestDto
    {
        [Required(ErrorMessage = "Allocation ID is required.")]
        public int AllocationId { get; set; }
    }
}
