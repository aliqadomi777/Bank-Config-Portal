using System.Collections.Generic;
using WebPortal.Application.DTO.Service;

namespace WebPortal.Application.Interfaces
{
    public interface IServiceManager
    {
        IEnumerable<ServiceResponseDto> GetAllServices(int bankId);
        ServiceResponseDto GetServiceById(int serviceId);
        int CreateService(ServiceCreateRequestDto request);
        bool UpdateService(ServiceUpdateRequestDto request);
        bool DeleteService(int serviceId);
    }
}
