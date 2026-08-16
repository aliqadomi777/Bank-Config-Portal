using System;
using System.ComponentModel.DataAnnotations;

namespace WebPortal.ASP.Models
{
    public class BranchViewModel
    {
        public int BranchId { get; set; }

        [Required(
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = "BranchNameENRequired")]
        [MaxLength(100,
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = "BranchNameENMaxLength")]
        [RegularExpression(@"^[a-zA-Z0-9.\-_ /]+$",
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = "BranchNameENInvalidCharacters")]
        public string BranchNameEN { get; set; }

        [Required(
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = "BranchNameARRequired")]
        [MaxLength(100,
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = "BranchNameARMaxLength")]
        [RegularExpression(@"^[\u0600-\u06FF0-9.\-_ /]+$",
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = "BranchNameARInvalidCharacters")]
        public string BranchNameAR { get; set; }

        [Required(
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = "BranchStatusRequired")]
        public bool BranchStatus { get; set; }

        public DateTimeOffset ModifiedAt { get; set; }

        public int BankId { get; set; }

    }
}
