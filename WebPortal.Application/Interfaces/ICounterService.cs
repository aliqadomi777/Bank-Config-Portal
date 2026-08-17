using System.Collections.Generic;
using WebPortal.Application.DTO.Counter;

namespace WebPortal.Application.Interfaces
{
    public interface ICounterService
    {
        IEnumerable<CounterResponseDto> GetAllCounters(int branchId, int bankId);
        CounterResponseDto GetCounterById(int counterId, int bankId);
        int CreateCounter(CounterCreateRequestDto request, int bankId);
        bool UpdateCounter(CounterUpdateRequestDto request, int bankId);
        bool DeleteCounter(int counterId, int bankId);
    }
}
