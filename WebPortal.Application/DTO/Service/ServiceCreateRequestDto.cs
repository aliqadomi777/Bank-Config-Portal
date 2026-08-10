using System.ComponentModel.DataAnnotations;

namespace WebPortal.Application.DTO.Service
{
    public class ServiceCreateRequestDto : ServiceBaseRequestDto
    {
        [Required(ErrorMessage = "Bank ID is required.")]

        public int BankId { get; set; }

    }
}
