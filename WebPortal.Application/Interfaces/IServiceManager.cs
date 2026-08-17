using System.Collections.Generic;
using WebPortal.Application.DTO.Service;

namespace WebPortal.Application.Interfaces
{
    public interface IServiceManager
    {
        IEnumerable<ServiceResponseDto> GetAllServices(int bankId);
        ServiceResponseDto GetServiceById(int serviceId, int bankId);
        int CreateService(ServiceCreateRequestDto request, int bankId);
        bool UpdateService(ServiceUpdateRequestDto request, int bankId);
        bool DeleteService(int serviceId, int bankId);

    }
}
