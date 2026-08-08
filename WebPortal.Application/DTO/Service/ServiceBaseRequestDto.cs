namespace WebPortal.Application.DTO.Service
{
    public class ServiceBaseRequestDto
    {
        public string ServiceNameEN { get; set; }
        public string ServiceNameAR { get; set; }
        public bool ServiceStatus { get; set; }
        public int MaxTicketsPerDay { get; set; }
    }
}
