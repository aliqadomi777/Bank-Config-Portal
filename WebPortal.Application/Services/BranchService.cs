using App.Shared;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using WebPortal.Application.DTO.Branch;
using WebPortal.Application.Exceptions;
using WebPortal.Application.Interfaces;
using WebPortal.Domain.Interfaces;
using WebPortal.Domain.Model;
namespace WebPortal.Application.Services
{
    public class BranchService : IBranchService
    {
        private readonly IFetchableRepository<BranchModel> _fetchRepository;
        private readonly IListableRepository<BranchModel> _listRepository;
        private readonly IAddableRepository<BranchModel> _addRepository;
        private readonly IUpdateableRepository<BranchModel> _updateRepository;
        private readonly IDeleteableRepository<BranchModel> _deleteRepository;
        private readonly ILogger<BranchModel> _logger;
        public BranchService(IFetchableRepository<BranchModel> fetchRepository,
                             IListableRepository<BranchModel> listRepository,
                             IAddableRepository<BranchModel> addRepository,
                             IUpdateableRepository<BranchModel> updateRepository,
                             IDeleteableRepository<BranchModel> deleteRepository,
                             ILogger<BranchModel> logger)
        {
            _fetchRepository = fetchRepository;
            _listRepository = listRepository;
            _addRepository = addRepository;
            _updateRepository = updateRepository;
            _deleteRepository = deleteRepository;
            _logger = logger;
        }

        public BranchResponseDto GetBranchById(int branchId)
        {
            if (branchId <= 0)
            {
                throw new ArgumentException("Branch ID must be a positive non-zero integer.", nameof(branchId));
            }

            try
            {
                var branch = _fetchRepository.GetById(branchId);
                return branch == null ? null : new BranchResponseDto
                {
                    BranchId = branch.BranchId,
                    BranchNameEN = branch.BranchNameEN,
                    BranchNameAR = branch.BranchNameAR,
                    BankId = branch.BranchId,
                    BranchStatus = branch.BranchStatus,
                    ModifiedAt = branch.ModifiedAt,
                };
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex,
                          ex.Message,
                          ex.Number,
                          branchId);
                throw;
            }


            catch (Exception ex)
            {
                _logger.LogError(ex,
                    ex.Message,
                    branchId);
                throw;

            }
        }

        public IEnumerable<BranchResponseDto> GetAllBranches(int bankId)
        {
            if (bankId <= 0)
            {
                throw new ArgumentException("Bank ID must be a positive non-zero integer.", nameof(bankId));
            }
            try
            {
                var branches = _listRepository.GetAll(bankId);
                return branches.Select(branch => new BranchResponseDto
                {
                    BranchId = branch.BranchId,
                    BranchNameEN = branch.BranchNameEN,
                    BranchNameAR = branch.BranchNameAR,
                    BankId = branch.BranchId,
                    BranchStatus = branch.BranchStatus,
                    ModifiedAt = branch.ModifiedAt,
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
        public int CreateBranch(BranchCreateRequestDto request)
        {
            try
            {
                ValidationExtensions.ValidateModel(request);
                var branchModel = new BranchModel
                {
                    BranchNameEN = request.BranchNameEN,
                    BranchNameAR = request.BranchNameAR,
                    BranchStatus = request.BranchStatus,
                    BankId = request.BankId

                };
                int newBranchId = _addRepository.Add(branchModel);
                return newBranchId;
            }
            catch (ValidationException)
            {
                return 0;
            }
            catch (SqlException ex) when (ex.Number == (int)SqlErrorTypes.UniqueConstraintViolation)
            {
                throw new DuplicateRecordException(
                    $"A Branch with the same English/Arabic name already exists",
                    ex);
            }
            catch (SqlException ex) when (ex.Number == (int)SqlErrorTypes.ForeignKeyViolation)
            {
                throw new ParentDeletedWithChildConflictException(
                    $"The bank you are adding the branch to, has been deleted.",
                    ex);
            }

            catch (SqlException ex)
            {
                _logger.LogError(ex,
                          ex.Message,
                          ex.Number,
                          request.BranchNameEN);

                throw;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    ex.Message,
                    request.BranchNameEN);

                throw;

            }
        }


        public bool UpdateBranch(BranchUpdateRequestDto request)
        {
            try
            {
                ValidationExtensions.ValidateModel(request);
                var branchModel = new BranchModel
                {
                    BranchId = request.BranchId,
                    BranchNameEN = request.BranchNameEN,
                    BranchNameAR = request.BranchNameAR,
                    BranchStatus = request.BranchStatus,
                };
                bool isUpdated = _updateRepository.Update(branchModel);
                return isUpdated;
            }
            catch (ValidationException)
            {
                return false;
            }
            catch (SqlException ex) when (ex.Number == (int)SqlErrorTypes.UniqueConstraintViolation)
            {
                throw new DuplicateRecordException(
                    $"A branch with the same English/Arabic name already exists",
                    ex);
            }
            catch (SqlException ex) when (ex.Number == (int)SqlErrorTypes.ForeignKeyViolation)
            {
                throw new ParentDeletedWithChildConflictException(
                    $"The bank you are adding the branch to, has been deleted.",
                    ex);
            }

            catch (SqlException ex)
            {
                _logger.LogError(ex,
                          ex.Message,
                          ex.Number,
                          request.BranchNameEN);

                throw;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    ex.Message,
                    request.BranchNameEN);

                throw;

            }
        }
        public bool DeleteBranch(int branchId)
        {
            try
            {
                bool isDeleted = _deleteRepository.Delete(branchId);
                return isDeleted;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex,
                          ex.Message,
                          ex.Number,
                          branchId);

                throw;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    ex.Message,
                    branchId);

                throw;

            }
        }

    }
}
