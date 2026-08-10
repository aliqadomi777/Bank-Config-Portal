using System.ComponentModel.DataAnnotations;

namespace WebPortal.Application.DTO.Service
{
    public class ServiceUpdateRequestDto : ServiceBaseRequestDto
    {
        [Required(ErrorMessage = "Service ID is required.")]

        public int ServiceId { get; set; }

    }
}
