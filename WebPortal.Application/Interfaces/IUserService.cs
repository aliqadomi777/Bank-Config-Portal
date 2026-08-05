using WebPortal.Application.DTO.User;

namespace WebPortal.Application.Interfaces
{
    public interface IUserService
    {
        UserResponseDto Login(UserRequestDto request);
    }
}
