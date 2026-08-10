using System.ComponentModel.DataAnnotations;

namespace WebPortal.Application.DTO.User
{
    public class UserRequestDto
    {
        [Required(ErrorMessage = "Username is required.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Bank name is required.")]
        public string BankName { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }
    }
}
