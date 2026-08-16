using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using WebPortal.Application.DTO.CounterType;
using WebPortal.Application.Interfaces;
using WebPortal.Domain.Interfaces;
using WebPortal.Domain.Model;

namespace WebPortal.Application.Services
{
    public class CounterTypeService : ICounterTypeService
    {
        private readonly IGetAllRepository<CounterTypeModel> _fetchAllRepository;
        private readonly ILogger<CounterTypeModel> _logger;
        public CounterTypeService(IGetAllRepository<CounterTypeModel> fetchAllRepository, ILogger<CounterTypeModel> logger)
        {
            _fetchAllRepository = fetchAllRepository;
            _logger = logger;
        }
        public IEnumerable<CounterTypeResponseDto> GetAllCounterTypes()
        {
            try
            {
                var counterTypes = _fetchAllRepository.GetAll();
                return counterTypes.Select(counterType => new CounterTypeResponseDto
                {
                    TypeId = counterType.TypeID,
                    TypeName = counterType.TypeName
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
    }
}
