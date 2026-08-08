using System;
using System.Data.SqlClient;
using WebPortal.Application.DTO.User;
using WebPortal.Application.Interfaces;
using WebPortal.Domain.Interfaces;
using WebPortal.Domain.Model;

namespace WebPortal.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IFetchableByBankUserRepository<UserModel> _fetchRepository;
        //private readonly ILogger<UserModel> _logger;
        public UserService(IFetchableByBankUserRepository<UserModel> fetchRepository)
        {
            _fetchRepository = fetchRepository;
            //_logger = logger;
        }
        public UserResponseDto Login(UserRequestDto request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            try
            {
                var details = _fetchRepository.GetByName(request.BankName.Trim(), request.UserName.Trim());

                if (details == null)
                {
                    throw new UnauthorizedAccessException("Invalid credentials.");
                }

                if (request.Password.Trim() != details.Password.Trim() ||
                    request.UserName.Trim() != details.UserName.Trim())
                {
                    throw new UnauthorizedAccessException("Invalid credentials.");
                }

                return new UserResponseDto
                {
                    BankId = details.BankId,
                    BankName = details.BankName,
                    UserId = details.UserId,
                    UserName = details.UserName,
                };
            }

            catch (SqlException ex)
            {
                //_logger.LogError(ex, ex.Message, ex.Number);
                throw;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, ex.Message);
                throw;
            }


        }

    }
}
