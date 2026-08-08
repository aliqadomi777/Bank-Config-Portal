using System;

namespace WebPortal.Domain.Model
{
    public class BranchModel
    {
        public int BranchId { get; set; }
        public string BranchNameEN { get; set; }
        public string BranchNameAR { get; set; }
        public bool BranchStatus { get; set; }
        public DateTimeOffset ModifiedAt { get; set; }
        public int BankId { get; set; }
    }
}
