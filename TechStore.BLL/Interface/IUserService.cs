using TechStore.DAL.Extensions;
using TechStore.Domain.DTOs.User;
using TechStore.Domain.Entities;

namespace TechStore.BLL.Interface;

public interface IUserService
{
    Task<Result<UserProfileResponseDto>> GetProfileAsync(int userId);
    Task<Result<UserProfileResponseDto>> UpdateProfileAsync(int userId, UpdateProfileRequestDto request);
    Task<Result<IReadOnlyList<AddressResponseDto>>> GetAddressesAsync(int userId);
    Task<Result<AddressResponseDto>> GetAddressByIdAsync(int userId, int addressId);
    Task<Result<AddressResponseDto>> AddAddressAsync(int userId, AddAddressRequestDto request);
    Task<Result<AddressResponseDto>> UpdateAddressAsync(int userId, UpdateAddressRequestDto request);
    Task<Result> DeleteAddressAsync(int userId, int addressId);
    Task<Result> SetDefaultAddressAsync(int userId, SetDefaultAddressRequestDto request);

}

