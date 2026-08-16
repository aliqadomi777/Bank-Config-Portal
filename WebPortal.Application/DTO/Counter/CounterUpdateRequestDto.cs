using System.ComponentModel.DataAnnotations;

namespace WebPortal.Application.DTO.Counter
{
    public class CounterUpdateRequestDto : CounterBaseRequestDto
    {
        [Required(ErrorMessage = "Counter ID is required.")]

        public int CounterId { get; set; }
    }
}
