using App.Shared;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using WebPortal.Application.DTO.Service;
using WebPortal.Application.Exceptions;
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
        private readonly ILogger<ServiceModel> _logger;
        private readonly IBankAuthorizationService _bankAuthorization;

        public ServiceManager(IFetchableRepository<ServiceModel> fetchRepository,
                             IListableRepository<ServiceModel> listRepository,
                             IAddableRepository<ServiceModel> addRepository,
                             IUpdateableRepository<ServiceModel> updateRepository,
                             IDeleteableRepository<ServiceModel> deleteRepository,
                             ILogger<ServiceModel> logger,
                             IBankAuthorizationService bankAuthorization)
        {
            _fetchRepository = fetchRepository;
            _listRepository = listRepository;
            _addRepository = addRepository;
            _updateRepository = updateRepository;
            _deleteRepository = deleteRepository;
            _logger = logger;
            _bankAuthorization = bankAuthorization;
        }

        public ServiceResponseDto GetServiceById(int serviceId, int bankId)
        {
            if (serviceId <= 0)
            {
                throw new ArgumentException("Service ID must be a positive non-zero integer.", nameof(serviceId));
            }

            try
            {
                var service = _bankAuthorization.GetServiceForBank(serviceId, bankId);

                return service == null ? null : new ServiceResponseDto
                {
                    ServiceId = service.ServiceId,
                    ServiceNameEN = service.ServiceNameEN,
                    ServiceNameAR = service.ServiceNameAR,
                    ServiceStatus = service.ServiceStatus,
                    MaxTicketsPerDay = service.MaxTicketsPerDay,
                    ModifiedAt = service.ModifiedAt,
                    BankId = service.BankId,
                    MaximumServiceTime = service.MaximumServiceTime,
                    MinimumServiceTime = service.MinimumServiceTime,
                };
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex,
                          ex.Message,
                          ex.Number,
                          serviceId);
                throw;
            }


            catch (Exception ex)
            {
                _logger.LogError(ex,
                    ex.Message,
                    serviceId);
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
                    BankId = service.BankId,
                    MaximumServiceTime = service.MaximumServiceTime,
                    MinimumServiceTime = service.MinimumServiceTime
                }).ToList();
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex,
                          ex.Message,
                          ex.Number,
                          bankId);
                throw;

            }


            catch (Exception ex)
            {
                _logger.LogError(ex,
                    ex.Message,
                    bankId);
                throw;

            }
        }

        public int CreateService(ServiceCreateRequestDto request, int bankId)
        {
            try
            {
                ValidationExtensions.ValidateModel(request);
                var serviceModel = new ServiceModel
                {
                    ServiceNameEN = request.ServiceNameEN,
                    ServiceNameAR = request.ServiceNameAR,
                    ServiceStatus = request.ServiceStatus,
                    MaxTicketsPerDay = request.MaxTicketsPerDay,
                    BankId = bankId,
                    MaximumServiceTime = request.MaximumServiceTime,
                    MinimumServiceTime = request.MinimumServiceTime

                };
                int newServiceId = _addRepository.Add(serviceModel);
                return newServiceId;
            }
            catch (ValidationException)
            {
                return 0;
            }
            catch (SqlException ex) when (ex.Number == (int)SqlErrorTypes.UniqueConstraintViolation)
            {
                throw new DuplicateRecordException(
                    $"A service with the same English/Arabic name already exists",
                    ex);
            }
            catch (SqlException ex) when (ex.Number == (int)SqlErrorTypes.ForeignKeyViolation)
            {
                throw new ParentDeletedWithChildConflictException(
                    $"The bank you are adding the service to, has been deleted.",
                    ex);
            }

            catch (SqlException ex)
            {
                _logger.LogError(ex,
                          ex.Message,
                          ex.Number,
                          request.ServiceNameEN);

                throw;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    ex.Message,
                    request.ServiceNameEN);

                throw;

            }
        }

        public bool UpdateService(ServiceUpdateRequestDto request, int bankId)
        {
            try
            {
                var currentService = _bankAuthorization.GetServiceForBank(request.ServiceId, bankId);

                if (currentService == null)
                {
                    return false;
                }

                ValidationExtensions.ValidateModel(request);
                var serviceModel = new ServiceModel
                {
                    ServiceId = request.ServiceId,
                    ServiceNameEN = request.ServiceNameEN,
                    ServiceNameAR = request.ServiceNameAR,
                    ServiceStatus = request.ServiceStatus,
                    MaxTicketsPerDay = request.MaxTicketsPerDay,
                    MaximumServiceTime = request.MaximumServiceTime,
                    MinimumServiceTime = request.MinimumServiceTime,

                };
                bool isUpdated = _updateRepository.Update(serviceModel);
                return isUpdated;
            }
            catch (ValidationException)
            {
                return false;
            }
            catch (SqlException ex) when (ex.Number == (int)SqlErrorTypes.UniqueConstraintViolation)
            {
                throw new DuplicateRecordException(
                    $"A service with the same English/Arabic name already exists",
                    ex);
            }
            catch (SqlException ex) when (ex.Number == (int)SqlErrorTypes.ForeignKeyViolation)
            {
                throw new ParentDeletedWithChildConflictException(
                    $"The bank you are updating the service on, has been deleted.",
                    ex);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex,
                          ex.Message,
                          ex.Number,
                          request.ServiceNameEN);

                throw;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    ex.Message,
                    request.ServiceNameEN);

                throw;

            }
        }

        public bool DeleteService(int serviceId, int bankId)
        {
            try
            {
                var service = _bankAuthorization.GetServiceForBank(serviceId, bankId);

                if (service == null)
                {
                    return false;
                }

                bool isDeleted = _deleteRepository.Delete(serviceId);
                return isDeleted;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex,
                          ex.Message,
                          ex.Number,
                          serviceId);

                throw;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    ex.Message,
                    serviceId);

                throw;

            }
        }

    }
}