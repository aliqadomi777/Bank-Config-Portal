using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebPortal.ASP.Models
{
    public class CounterViewModel
    {
        public int CounterId { get; set; }

        public int BranchId { get; set; }

        [Required(
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = "CounterNameENRequired")]
        [MaxLength(100,
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = "CounterNameENMaxLength")]
        [RegularExpression(@"^[a-zA-Z0-9.\-_ /]+$",
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = "CounterNameENInvalidCharacters")]

        public string CounterNameEN { get; set; }

        [Required(
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = "CounterNameARRequired")]
        [MaxLength(100,
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = "CounterNameARMaxLength")]
        [RegularExpression(@"^[\u0600-\u06FF0-9.\-_ /]+$",
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = "CounterNameARInvalidCharacters")]
        public string CounterNameAR { get; set; }
        [Required(
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = "CounterStatusRequired")]
        public bool CounterStatus { get; set; }

        [Required(
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = "CounterTypeIdRequired")]

        public int CounterTypeId { get; set; }

        public string CounterTypeName { get; set; }

        public DateTimeOffset ModifiedAt { get; set; }

        public List<CounterTypeViewModel> Types { get; set; }
    }
}