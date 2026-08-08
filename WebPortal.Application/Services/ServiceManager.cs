using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using WebPortal.Application.DTO.Service;
using WebPortal.Application.Interfaces;
using WebPortal.Domain.Interfaces;
using WebPortal.Domain.Model;

namespace WebPortal.Application.Services
{
    public class ServiceManager : IServiceManager
    {
        private readonly IFetchableRepository<ServiceModel> _fetchRepository;
        private readonly IListableRepository<ServiceModel> _listRepository;
        private readonly IAddableRepository<ServiceModel> _addRepository;
        private readonly IUpdateableRepository<ServiceModel> _updateRepository;
        private readonly IDeleteableRepository<ServiceModel> _deleteRepository;
        //private readonly ILogger<BranchModel> _logger;
        public ServiceManager(IFetchableRepository<ServiceModel> fetchRepository,
                             IListableRepository<ServiceModel> listRepository,
                             IAddableRepository<ServiceModel> addRepository,
                             IUpdateableRepository<ServiceModel> updateRepository,
                             IDeleteableRepository<ServiceModel> deleteRepository)
        {
            _fetchRepository = fetchRepository;
            _listRepository = listRepository;
            _addRepository = addRepository;
            _updateRepository = updateRepository;
            _deleteRepository = deleteRepository;
            //_logger = logger;
        }
        public ServiceResponseDto GetServiceById(int serviceId)
        {
            if (serviceId <= 0)
            {
                throw new ArgumentException("Service ID must be a positive non-zero integer.", nameof(serviceId));
            }

            try
            {
                var service = _fetchRepository.GetById(serviceId);
                return service == null ? null : new ServiceResponseDto
                {
                    ServiceId = service.ServiceId,
                    ServiceNameEN = service.ServiceNameEN,
                    ServiceNameAR = service.ServiceNameAR,
                    ServiceStatus = service.ServiceStatus,
                    MaxTicketsPerDay = service.MaxTicketsPerDay,
                    ModifiedAt = service.ModifiedAt,
                    BankId = service.BankId
                };
            }
            catch (SqlException ex)
            {
                //_logger.LogError(ex,
                //          ex.Message,
                //          ex.Number,
                //          serviceId);
                throw;
            }


            catch (Exception ex)
            {
                //_logger.LogError(ex,
                //    ex.Message,
                //    serviceId);
                throw;

            }
        }
        public IEnumerable<ServiceResponseDto> GetAllServices(int bankId)
        {
            if (bankId <= 0)
            {
                throw new ArgumentException("Bank ID must be a positive non-zero integer.", nameof(bankId));
            }
            try
            {
                var services = _listRepository.GetAll(bankId);
                return services.Select(service => new ServiceResponseDto
                {
                    ServiceId = service.ServiceId,
                    ServiceNameEN = service.ServiceNameEN,
                    ServiceNameAR = service.ServiceNameAR,
                    ServiceStatus = service.ServiceStatus,
                    MaxTicketsPerDay = service.MaxTicketsPerDay,
                    ModifiedAt = service.ModifiedAt,
                    BankId = service.BankId
                }).ToList();
            }
            catch (SqlException ex)
            {
                //_logger.LogError(ex,
                //          ex.Message,
                //          ex.Number,
                //          bankId);
                throw;

            }


            catch (Exception ex)
            {
                //_logger.LogError(ex,
                //    ex.Message,
                //    bankId);
                throw;

            }
        }

        public int CreateService(ServiceCreateRequestDto request)
        {
            try
            {
                var serviceModel = new ServiceModel
                {
                    ServiceNameEN = request.ServiceNameEN,
                    ServiceNameAR = request.ServiceNameAR,
                    ServiceStatus = request.ServiceStatus,
                    MaxTicketsPerDay = request.MaxTicketsPerDay,
                    BankId = request.BankId

                };
                int newServiceId = _addRepository.Add(serviceModel);
                return newServiceId;
            }

            catch (SqlException ex)
            {
                //_logger.LogError(ex,
                //          ex.Message,
                //          ex.Number,
                //          request.ServiceNameEN);

                throw;

            }
            catch (Exception ex)
            {
                //_logger.LogError(ex,
                //    ex.Message,
                //    request.ServiceNameEN);

                throw;

            }
        }

        public bool UpdateService(ServiceUpdateRequestDto request)
        {
            try
            {
                var serviceModel = new ServiceModel
                {
                    ServiceId = request.ServiceId,
                    ServiceNameEN = request.ServiceNameEN,
                    ServiceNameAR = request.ServiceNameAR,
                    ServiceStatus = request.ServiceStatus,
                    MaxTicketsPerDay = request.MaxTicketsPerDay,

                };
                bool isUpdated = _updateRepository.Update(serviceModel);
                return isUpdated;
            }
            catch (SqlException ex)
            {
                //_logger.LogError(ex,
                //          ex.Message,
                //          ex.Number,
                //          request.ServiceNameEN);

                throw;

            }
            catch (Exception ex)
            {
                //_logger.LogError(ex,
                //    ex.Message,
                //    request.ServiceNameEN);

                throw;

            }
        }
        public bool DeleteService(int serviceId)
        {
            try
            {
                bool isDeleted = _deleteRepository.Delete(serviceId);
                return isDeleted;
            }
            catch (SqlException ex)
            {
                //_logger.LogError(ex,
                //          ex.Message,
                //          ex.Number,
                //          serviceId);

                throw;

            }
            catch (Exception ex)
            {
                //_logger.LogError(ex,
                //    ex.Message,
                //    serviceId);

                throw;

            }
        }

    }
}
