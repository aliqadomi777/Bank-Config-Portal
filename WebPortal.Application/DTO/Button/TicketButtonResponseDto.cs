using WebPortal.Application.DTO.Service;

namespace WebPortal.Application.DTO.Button
{
    public class TicketButtonResponseDto : BaseButtonResponseDto
    {
        public int TicketId { get; set; }

        public ServiceResponseDto Service { get; set; }
    }
}
