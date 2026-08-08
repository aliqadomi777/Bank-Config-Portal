using System.Collections.Generic;

namespace WebPortal.ASP.Models
{
    public class DashboardViewModel
    {
        public List<BranchViewModel> Branches { get; set; }

        public List<ServiceViewModel> Services { get; set; }


        public DashboardViewModel()
        {
            Branches = new List<BranchViewModel>();
            Services = new List<ServiceViewModel>();
        }
    }
}