using System.ComponentModel.DataAnnotations;

namespace WebPortal.Application.DTO.Allocations
{
    public class AllocationBaseRequestDto
    {
        [Required(ErrorMessage = "Service ID is required.")]
        public int ServiceId { get; set; }

    }
}
