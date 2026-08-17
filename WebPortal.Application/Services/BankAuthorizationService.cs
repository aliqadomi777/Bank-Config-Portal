using System;
using WebPortal.Application.Interfaces;
using WebPortal.Domain.Interfaces;
using WebPortal.Domain.Model;

namespace WebPortal.Application.Services
{
    public class BankAuthorizationService
        : IBankAuthorizationService
    {
        private readonly IFetchableRepository<ServiceModel>
            _serviceRepository;

        private readonly IFetchableRepository<BranchModel>
            _branchRepository;

        private readonly IFetchableRepository<CounterModel>
            _counterRepository;

        private readonly IFetchableRepository<AllocationModel>
            _allocationRepository;


        public BankAuthorizationService(
            IFetchableRepository<ServiceModel> serviceRepository,
            IFetchableRepository<BranchModel> branchRepository,
            IFetchableRepository<CounterModel> counterRepository,
            IFetchableRepository<AllocationModel> allocationRepository)
        {
            _serviceRepository = serviceRepository;

            _branchRepository = branchRepository;

            _counterRepository = counterRepository;

            _allocationRepository = allocationRepository;
        }


        public ServiceModel GetServiceForBank(
            int serviceId,
            int bankId)
        {
            ValidateBankId(bankId);

            var service =
                _serviceRepository.GetById(serviceId);

            if (service == null)
            {
                return null;
            }

            if (service.BankId != bankId)
            {
                throw new UnauthorizedAccessException(
                    "The service does not belong to the current bank.");
            }

            return service;
        }


        public BranchModel GetBranchForBank(
            int branchId,
            int bankId)
        {
            ValidateBankId(bankId);

            var branch =
                _branchRepository.GetById(branchId);

            if (branch == null)
            {
                return null;
            }

            if (branch.BankId != bankId)
            {
                throw new UnauthorizedAccessException(
                    "The branch does not belong to the current bank.");
            }

            return branch;
        }


        public CounterModel GetCounterForBank(
            int counterId,
            int bankId)
        {
            ValidateBankId(bankId);

            var counter =
                _counterRepository.GetById(counterId);

            if (counter == null)
            {
                return null;
            }

            var branch =
                _branchRepository.GetById(
                    counter.BranchId);

            if (branch == null)
            {
                return null;
            }

            if (branch.BankId != bankId)
            {
                throw new UnauthorizedAccessException(
                    "The counter does not belong to the current bank.");
            }

            return counter;
        }


        public AllocationModel GetAllocationForBank(
            int allocationId,
            int bankId)
        {
            ValidateBankId(bankId);

            var allocation =
                _allocationRepository.GetById(
                    allocationId);

            if (allocation == null)
            {
                return null;
            }

            var counter =
                GetCounterForBank(
                    allocation.CounterId,
                    bankId);

            if (counter == null)
            {
                return null;
            }

            var service =
                GetServiceForBank(
                    allocation.ServiceId,
                    bankId);

            if (service == null)
            {
                return null;
            }

            return allocation;
        }


        private static void ValidateBankId(
            int bankId)
        {
            if (bankId <= 0)
            {
                throw new ArgumentException(
                    "Bank ID must be a positive non-zero integer.",
                    nameof(bankId));
            }
        }
    }
}