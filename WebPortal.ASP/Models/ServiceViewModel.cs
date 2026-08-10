using System;
using System.ComponentModel.DataAnnotations;
using WebPortal.ASP.Resources;

namespace WebPortal.ASP.Models
{
    public class ServiceViewModel
    {
        [Required(
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = "ServiceNameENRequired")]
        [MaxLength(100,
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = "ServiceNameENMaxLength")]
        [RegularExpression(@"^[a-zA-Z0-9.\-_ /]+$",
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = "ServiceNameENInvalidCharacters")]
        public string ServiceNameEN { get; set; }

        [Required(
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = "ServiceNameARRequired")]
        [MaxLength(100,
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = "ServiceNameARMaxLength")]
        [RegularExpression(@"^[\u0600-\u06FF0-9.\-_ /]+$",
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = "ServiceNameARInvalidCharacters")]
        public string ServiceNameAR { get; set; }

        [Required(
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = "ServiceStatusRequired")]
        public bool ServiceStatus { get; set; }

        [Required(
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = "MaxTicketsPerDayRequired")]
        [Range(1, 100,
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = "MaxTicketsPerDayInvalid")]
        public int MaxTicketsPerDay { get; set; }

        public int ServiceId { get; set; }

        public DateTimeOffset ModifiedAt { get; set; }
        public int BankId { get; set; }

    }
}
