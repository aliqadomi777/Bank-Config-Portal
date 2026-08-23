using System;
using System.ComponentModel.DataAnnotations;
using WebPortal.ASP.Validation;
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

        [Required(
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = "MinimumServiceTimerequired")]
        [Range(30, 999999,
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = "MinimumServiceTimeInvalid")]
        [ClientLessThan(
            "MaximumServiceTime",
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = "MinServiceTimeGreaterThanMax")]
        public int MinimumServiceTime { get; set; }

        [Required(
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = "MaximumServiceTimerequired")]
        [Range(30, 999999,
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = "MaximumServiceTimeInvalid")]
        public int MaximumServiceTime { get; set; }

    }
}
