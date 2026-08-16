using System.Collections.Generic;
using WebPortal.Application.DTO.CounterType;
namespace WebPortal.Application.Interfaces
{
    public interface ICounterTypeService
    {
        IEnumerable<CounterTypeResponseDto> GetAllCounterTypes();

    }
}
