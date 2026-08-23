using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using WebPortal.Application.Validation;

namespace WebPortal.Application.DTO.Service
{
    public class ServiceBaseRequestDto
    {
        [Required(ErrorMessage = "Service name is required.")]
        [MaxLength(100, ErrorMessage = "Service name can't exceed 100 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9.\-_ /]+$", ErrorMessage = "Service name contains invalid characters.")]
        public string ServiceNameEN { get; set; }

        [Required(ErrorMessage = "Arabic service name is required.")]
        [MaxLength(100, ErrorMessage = "Arabic service name can't exceed 100 characters.")]
        [RegularExpression(@"^[\u0600-\u06FF0-9.\-_ /]+$", ErrorMessage = "Arabic service name contains invalid characters.")]
        public string ServiceNameAR { get; set; }

        [Required(ErrorMessage = "Service status is required.")]
        public bool ServiceStatus { get; set; }

        [Required(ErrorMessage = "Maximum tickets per day is required.")]
        [Range(1, 100, ErrorMessage = "Maximum tickets per day must be between 1 and 100.")]
        public int MaxTicketsPerDay { get; set; }

        [Required(ErrorMessage = "Minimum Service Time is required.")]
        [Range(30, 999999, ErrorMessage = "Minimum Service Time must be between 30 and 999999 seconds.")]
        [LessThan("MaximumServiceTime", ErrorMessage = "Minimum Service Time must be less than Maximum Service Time.")]
        public int MinimumServiceTime { get; set; }

        [Required(ErrorMessage = "Maximum Service Time is required.")]
        [Range(30, 999999, ErrorMessage = "Maximum Service Time must be between 30 and 999999 seconds.")]
        public int MaximumServiceTime { get; set; }

    }
}
