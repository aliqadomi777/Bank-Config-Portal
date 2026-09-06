using App.Domain.Models;

namespace WebPortal.Domain.Interfaces
{
    public interface ITicketingScreenRepository
    {
        ScreenModel GetActiveScreen(int branchId, int bankId);
    }
}