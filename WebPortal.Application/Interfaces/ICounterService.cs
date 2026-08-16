using System.Collections.Generic;
using WebPortal.Application.DTO.Counter;

namespace WebPortal.Application.Interfaces
{
    public interface ICounterService
    {
        IEnumerable<CounterResponseDto> GetAllCounters(int branchId);
        CounterResponseDto GetCounterById(int counterId);
        int CreateCounter(CounterCreateRequestDto request);
        bool UpdateCounter(CounterUpdateRequestDto request);
        bool DeleteCounter(int counterId);
    }
}
