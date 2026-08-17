using App.Shared;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using WebPortal.Application.DTO.Counter;
using WebPortal.Application.Exceptions;
using WebPortal.Application.Interfaces;
using WebPortal.Domain.Interfaces;
using WebPortal.Domain.Model;

namespace WebPortal.Application.Services
{
    public class CounterService : ICounterService
    {
        private readonly IFetchableRepository<CounterModel> _fetchRepository;
        private readonly IListableRepository<CounterModel> _listRepository;
        private readonly IAddableRepository<CounterModel> _addRepository;
        private readonly IUpdateableRepository<CounterModel> _updateRepository;
        private readonly IDeleteableRepository<CounterModel> _deleteRepository;
        private readonly ILogger<CounterModel> _logger;

        private readonly IListableRepository<AllocationModel> _listAllocationRepository;
        private readonly IAllocationRepository _deleteAllocationRepository;
        private readonly IBankAuthorizationService _bankAuthorization;

        public CounterService(IFetchableRepository<CounterModel> fetchRepository,
                     IListableRepository<CounterModel> listRepository,
                     IAddableRepository<CounterModel> addRepository,
                     IUpdateableRepository<CounterModel> updateRepository,
                     IDeleteableRepository<CounterModel> deleteRepository,
                     ILogger<CounterModel> logger,
                     IListableRepository<AllocationModel> listAllocationRepository,
                     IAllocationRepository deleteAllocationRepository,
                     IBankAuthorizationService bankAuthorization)
        {
            _fetchRepository = fetchRepository;
            _listRepository = listRepository;
            _addRepository = addRepository;
            _updateRepository = updateRepository;
            _deleteRepository = deleteRepository;
            _logger = logger;
            _listAllocationRepository = listAllocationRepository;
            _deleteAllocationRepository = deleteAllocationRepository;
            _bankAuthorization = bankAuthorization;
        }

        public CounterResponseDto GetCounterById(int counterId, int bankId)
        {
            if (counterId <= 0)
            {
                throw new ArgumentException("Counter ID must be a positive non-zero integer.", nameof(counterId));
            }

            try
            {
                var counter = _bankAuthorization.GetCounterForBank(counterId, bankId);

                return counter == null ? null : new CounterResponseDto
                {
                    CounterId = counter.CounterId,
                    CounterNameEN = counter.CounterNameEN,
                    CounterNameAR = counter.CounterNameAR,
                    CounterStatus = counter.CounterStatus,
                    BranchId = counter.BranchId,
                    ModifiedAt = counter.ModifiedAt,
                    TypeID = counter.TypeID,
                    TypeName = counter.TypeName
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

        public IEnumerable<CounterResponseDto> GetAllCounters(int branchId, int bankId)
        {
            if (branchId <= 0)
            {
                throw new ArgumentException("Branch ID must be a positive non-zero integer.", nameof(branchId));
            }
            try
            {
                var branch = _bankAuthorization.GetBranchForBank(branchId, bankId);

                if (branch == null)
                {
                    return Enumerable.Empty<CounterResponseDto>();
                }

                var counters = _listRepository.GetAll(branchId);

                return counters.Select(counter => new CounterResponseDto
                {
                    CounterId = counter.CounterId,
                    CounterNameEN = counter.CounterNameEN,
                    CounterNameAR = counter.CounterNameAR,
                    CounterStatus = counter.CounterStatus,
                    BranchId = counter.BranchId,
                    ModifiedAt = counter.ModifiedAt,
                    TypeID = counter.TypeID,
                    TypeName = counter.TypeName
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

        public int CreateCounter(CounterCreateRequestDto request, int bankId)
        {
            try
            {
                _bankAuthorization.GetBranchForBank(request.BranchId, bankId);

                ValidationExtensions.ValidateModel(request);
                var counterModel = new CounterModel
                {

                    BranchId = request.BranchId,
                    CounterNameEN = request.CounterNameEN,
                    CounterNameAR = request.CounterNameAR,
                    CounterStatus = request.CounterStatus,
                    TypeID = request.TypeID

                };
                int newCounterId = _addRepository.Add(counterModel);
                return newCounterId;
            }
            catch (ValidationException)
            {
                return 0;
            }
            catch (SqlException ex) when (ex.Number == (int)SqlErrorTypes.UniqueConstraintViolation)
            {
                throw new DuplicateRecordException(
                    $"A Counter with the same English/Arabic name already exists",
                    ex);
            }
            catch (SqlException ex) when (ex.Number == (int)SqlErrorTypes.ForeignKeyViolation)
            {
                throw new ParentDeletedWithChildConflictException(
                    $"The branch you are adding the counter to, has been deleted.",
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

        public bool UpdateCounter(CounterUpdateRequestDto request, int bankId)
        {
            try
            {
                var currentCounter = _bankAuthorization.GetCounterForBank(request.CounterId, bankId);

                if (currentCounter == null)
                {
                    return false;
                }

                ValidationExtensions.ValidateModel(request);
                var counterModel = new CounterModel
                {
                    CounterId = request.CounterId,
                    CounterNameEN = request.CounterNameEN,
                    CounterNameAR = request.CounterNameAR,
                    TypeID = request.TypeID,
                    CounterStatus = request.CounterStatus
                };
                bool isUpdated = _updateRepository.Update(counterModel);
                return isUpdated;
            }
            catch (ValidationException)
            {
                return false;
            }
            catch (SqlException ex) when (ex.Number == (int)SqlErrorTypes.UniqueConstraintViolation)
            {
                throw new DuplicateRecordException(
                    $"A Counter with the same English/Arabic name already exists",
                    ex);
            }
            catch (SqlException ex) when (ex.Number == (int)SqlErrorTypes.ForeignKeyViolation)
            {
                throw new ParentDeletedWithChildConflictException(
                    $"The branch you are updating the counter on, has been deleted.",
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

        public bool DeleteCounter(int counterId, int bankId)
        {
            try
            {
                var counter = _bankAuthorization.GetCounterForBank(counterId, bankId);

                if (counter == null)
                {
                    return false;
                }

                using (var scope = new System.Transactions.TransactionScope(
                    System.Transactions.TransactionScopeOption.Required,
                    new System.Transactions.TransactionOptions
                    {
                        IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
                    }))
                {
                    _deleteAllocationRepository.DeleteAll(counterId);
                    bool isCounterDeleted = _deleteRepository.Delete(counterId);

                    scope.Complete();

                    return isCounterDeleted;
                }
            }
            catch (SqlException ex)
            {
                _logger.LogError(
                    ex,
                    ex.Message,
                    ex.Number);

                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    ex.Message);

                throw;
            }
        }


    }
}