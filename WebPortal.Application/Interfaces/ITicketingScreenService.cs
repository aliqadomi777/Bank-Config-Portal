using WebPortal.Application.DTO.Screen;

namespace WebPortal.Application.Interfaces
{
    public interface ITicketingScreenService
    {
        ScreenResponseDto GetActiveScreen(int branchId, int bankId);
    }
}