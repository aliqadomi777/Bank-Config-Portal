namespace WebPortal.API.Models
{
    public class TicketButtonResponseModel : BaseButtonResponseModel
    {
        public int TicketId { get; set; }

        public int ServiceId { get; set; }

        public string ServiceNameEN { get; set; }

        public string ServiceNameAR { get; set; }

        public int MaxTicketsPerDay { get; set; }

        public int MinimumServiceTime { get; set; }

        public int MaximumServiceTime { get; set; }
    }
}