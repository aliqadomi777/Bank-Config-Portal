using System;

namespace WebPortal.Application.DTO.Service
{
    public class ServiceResponseDto
    {
        public int ServiceId { get; set; }
        public string ServiceNameEN { get; set; }
        public string ServiceNameAR { get; set; }
        public bool ServiceStatus { get; set; }
        public DateTimeOffset ModifiedAt { get; set; }
        public int MaxTicketsPerDay { get; set; }
        public int MinimumServiceTime { get; set; }
        public int MaximumServiceTime { get; set; }
        public int BankId { get; set; }
    }
}
