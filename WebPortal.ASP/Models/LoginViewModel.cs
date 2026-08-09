using System.ComponentModel.DataAnnotations;
namespace WebPortal.ASP.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "RequiredError")]
        [Display(Name = "Username", ResourceType = typeof(Resources.Resources))]
        public string Username { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "RequiredError")]
        [DataType(DataType.Password)]
        [Display(Name = "Password", ResourceType = typeof(Resources.Resources))]
        public string Password { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "RequiredError")]
        [Display(Name = "BankName", ResourceType = typeof(Resources.Resources))]
        public string BankName { get; set; }
    }
}
