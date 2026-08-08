using System;

namespace WebPortal.ASP.Models
{
    public class ServiceViewModel
    {
        public int ServiceId { get; set; }
        public string ServiceNameEN { get; set; }
        public string ServiceNameAR { get; set; }
        public bool ServiceStatus { get; set; }
        public DateTimeOffset ModifiedAt { get; set; }
        public int MaxTicketsPerDay { get; set; }
        public int BankId { get; set; }
    }
}