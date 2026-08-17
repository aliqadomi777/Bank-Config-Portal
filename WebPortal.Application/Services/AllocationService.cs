using App.Shared;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using WebPortal.Application.DTO.Allocations;
using WebPortal.Application.Exceptions;
using WebPortal.Application.Interfaces;
using WebPortal.Domain.Interfaces;
using WebPortal.Domain.Model;

namespace WebPortal.Application.Services
{
    public class AllocationService : IAllocationService
    {
        private readonly IFetchableRepository<AllocationModel> _fetchRepository;
        private readonly IListableRepository<AllocationModel> _listRepository;
        private readonly IAddableRepository<AllocationModel> _addRepository;
        private readonly IUpdateableRepository<AllocationModel> _updateRepository;
        private readonly IDeleteableRepository<AllocationModel> _deleteRepository;
        private readonly ILogger<AllocationModel> _logger;
        private readonly IBankAuthorizationService _bankAuthorization;

        public AllocationService(IFetchableRepository<AllocationModel> fetchRepository,
                             IListableRepository<AllocationModel> listRepository,
                             IAddableRepository<AllocationModel> addRepository,
                             IUpdateableRepository<AllocationModel> updateRepository,
                             IDeleteableRepository<AllocationModel> deleteRepository,
                             ILogger<AllocationModel> logger,
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

        public AllocationResponseDto GetAllocationById(int allocationId, int bankId)
        {
            if (allocationId <= 0)
            {
                throw new ArgumentException("Allocation ID must be a positive non-zero integer.", nameof(allocationId));
            }

            try
            {
                var allocation = _bankAuthorization.GetAllocationForBank(allocationId, bankId);

                return allocation == null ? null : new AllocationResponseDto
                {
                    AllocationId = allocation.AllocationId,
                    ServiceNameEN = allocation.ServiceNameEN,
                    ServiceNameAR = allocation.ServiceNameAR,
                    ServiceId = allocation.ServiceId,
                    CounterId = allocation.CounterId
                };
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex,
                          ex.Message,
                          ex.Number);
                throw;
            }


            catch (Exception ex)
            {
                _logger.LogError(ex,
                    ex.Message);
                throw;

            }
        }

        public IEnumerable<AllocationResponseDto> GetAllAllocations(int counterId, int bankId)
        {
            if (counterId <= 0)
            {
                throw new ArgumentException("Counter ID must be a positive non-zero integer.", nameof(counterId));
            }
            try
            {
                var counter = _bankAuthorization.GetCounterForBank(counterId, bankId);

                if (counter == null)
                {
                    return Enumerable.Empty<AllocationResponseDto>();
                }

                var allocations = _listRepository.GetAll(counterId).ToList();

                foreach (var allocation in allocations)
                {
                    _bankAuthorization.GetAllocationForBank(allocation.AllocationId, bankId);
                }

                return allocations.Select(allocation => new AllocationResponseDto
                {
                    AllocationId = allocation.AllocationId,
                    ServiceNameEN = allocation.ServiceNameEN,
                    ServiceNameAR = allocation.ServiceNameAR,
                    ServiceId = allocation.ServiceId,
                    CounterId = allocation.CounterId
                }).ToList();
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex,
                          ex.Message,
                          ex.Number);
                throw;

            }


            catch (Exception ex)
            {
                _logger.LogError(ex,
                    ex.Message);
                throw;

            }
        }

        public int CreateAllocation(AllocationCreateRequestDto request, int bankId)
        {
            try
            {
                _bankAuthorization.GetCounterForBank(request.CounterId, bankId);
                _bankAuthorization.GetServiceForBank(request.ServiceId, bankId);

                ValidationExtensions.ValidateModel(request);
                var allocationModel = new AllocationModel
                {
                    CounterId = request.CounterId,
                    ServiceId = request.ServiceId
                };
                int newAllocationId = _addRepository.Add(allocationModel);
                return newAllocationId;
            }
            catch (ValidationException)
            {
                return 0;
            }
            catch (SqlException ex) when (ex.Number == (int)SqlErrorTypes.UniqueConstraintViolation)
            {
                throw new DuplicateRecordException(
                    $"The same service already assigned to this counter",
                    ex);
            }
            catch (SqlException ex) when (ex.Number == (int)SqlErrorTypes.ForeignKeyViolation)
            {
                throw new ParentDeletedWithChildConflictException(
                    $"The Counter you are adding the service to, has been deleted.",
                    ex);
            }

            catch (SqlException ex)
            {
                _logger.LogError(ex,
                          ex.Message,
                          ex.Number);

                throw;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    ex.Message);

                throw;

            }
        }



        public bool UpdateAllocation(AllocationUpdateRequestDto request, int bankId)
        {
            try
            {
                var currentAllocation = _bankAuthorization.GetAllocationForBank(request.AllocationId, bankId);

                if (currentAllocation == null)
                {
                    return false;
                }

                _bankAuthorization.GetServiceForBank(request.ServiceId, bankId);

                ValidationExtensions.ValidateModel(request);
                var allocationModel = new AllocationModel
                {
                    ServiceId = request.ServiceId,
                    AllocationId = request.AllocationId
                };
                bool isUpdated = _updateRepository.Update(allocationModel);
                return isUpdated;
            }
            catch (ValidationException)
            {
                return false;
            }
            catch (SqlException ex) when (ex.Number == (int)SqlErrorTypes.UniqueConstraintViolation)
            {
                throw new DuplicateRecordException(
                    $"The same service already assigned to this counter",
                    ex);
            }
            catch (SqlException ex) when (ex.Number == (int)SqlErrorTypes.ForeignKeyViolation)
            {
                throw new ParentDeletedWithChildConflictException(
                    $"The Counter you are updating the service on, has been deleted.",
                    ex);
            }

            catch (SqlException ex)
            {
                _logger.LogError(ex,
                          ex.Message,
                          ex.Number);

                throw;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    ex.Message);

                throw;

            }
        }

        public bool DeleteAllocation(int allocationId, int bankId)
        {
            try
            {
                var allocation = _bankAuthorization.GetAllocationForBank(allocationId, bankId);

                if (allocation == null)
                {
                    return false;
                }

                bool isDeleted = _deleteRepository.Delete(allocationId);
                return isDeleted;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex,
                          ex.Message,
                          ex.Number);

                throw;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    ex.Message);

                throw;

            }
        }
    }
}