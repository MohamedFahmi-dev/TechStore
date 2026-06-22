using TechStore.DAL.Extensions;
using TechStore.Domain.DTOs.Admin;

namespace TechStore.BLL.Interface;

public interface IAdminService
{
    Task<Result<IReadOnlyList<UserResponseDto>>> GetAllUsersAsync();
    Task<Result<UserResponseDto>> GetUserByIdAsync(int userId);
    Task<Result<UserResponseDto>> CreateUserAsync(CreateUserRequestDto request);
    Task<Result<UserResponseDto>> UpdateUserAsync(UpdateUserRequestDto request);
    Task<Result> AssignRoleAsync(AssignRoleRequestDto request);
    Task<Result> DeactivateUserAsync(int userId);
    Task<Result> ActivateUserAsync(int userId);
}
