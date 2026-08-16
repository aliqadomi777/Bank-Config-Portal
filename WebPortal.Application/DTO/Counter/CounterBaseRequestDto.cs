using System.ComponentModel.DataAnnotations;

namespace WebPortal.Application.DTO.Counter
{
    public class CounterBaseRequestDto
    {
        [Required(ErrorMessage = "Counter name is required.")]
        [MaxLength(100, ErrorMessage = "Counter name can't exceed 100 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9.\-_ /]+$", ErrorMessage = "Counter name contains invalid characters.")]
        public string CounterNameEN { get; set; }

        [Required(ErrorMessage = "Counter name is required.")]
        [MaxLength(100, ErrorMessage = "Counter name can't exceed 100 characters.")]
        [RegularExpression(@"^[\u0600-\u06FF0-9.\-_ /]+$", ErrorMessage = "Counter name contains invalid characters.")]
        public string CounterNameAR { get; set; }

        [Required(ErrorMessage = "Counter status is required.")]

        public bool CounterStatus { get; set; }

        [Required(ErrorMessage = "Counter Type ID is required.")]

        public int TypeID { get; set; }


    }
}
