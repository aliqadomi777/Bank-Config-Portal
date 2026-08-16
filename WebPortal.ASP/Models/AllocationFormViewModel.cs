using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebPortal.ASP.Models
{
    public class AllocationFormViewModel
    {
        public int CounterId { get; set; }

        [Range(
            1,
            int.MaxValue,
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = "ServiceRequired")]
        public int ServiceId { get; set; }

        public List<ServiceOptionViewModel> Services { get; set; }

        public AllocationFormViewModel()
        {
            Services = new List<ServiceOptionViewModel>();
        }
    }
}