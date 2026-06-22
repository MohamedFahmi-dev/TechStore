using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TechStore.BLL.Interface;
using TechStore.DAL.Context;
using TechStore.DAL.Extensions;
using TechStore.Domain.DTOs.User;
using TechStore.Domain.Entities;

using Microsoft.AspNetCore.Identity;
using TechStore.DAL.UnitOfWork;

namespace TechStore.BLL.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _uow;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;

    public UserService(IUnitOfWork uow, UserManager<ApplicationUser> userManager, IMapper mapper)
    {
        _uow = uow;
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<Result<AddressResponseDto>> AddAddressAsync(int userId, AddAddressRequestDto request)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return Error.Failure("User not found.");

        if (request.IsDefault)
        {
            var existingAddresses = await _uow.Addresses.FindAsync(a => a.UserId == userId);
            foreach (var existing in existingAddresses)
            {
                existing.IsDefault = false;
                _uow.Addresses.Update(existing);
            }
        }

        var address = new Address
        {
            UserId = userId,
            Label = request.Label,
            StreetAddress = request.StreetAddress,
            City = request.City,
            Governorate = request.Governorate,
            PhoneNumber = request.PhoneNumber,
            IsDefault = request.IsDefault
        };

        await _uow.Addresses.AddAsync(address);
        await _uow.SaveChangesAsync();

        return Result<AddressResponseDto>.Ok(_mapper.Map<AddressResponseDto>(address));
    }

    public async Task<Result> DeleteAddressAsync(int userId, int addressId)
    {
        var addresses = await _uow.Addresses.FindAsync(st => st.Id == addressId && st.UserId == userId);
        var address = addresses.FirstOrDefault();

        if (address == null)
            return Error.Failure("Address not found or unauthorized.");
            
        _uow.Addresses.Delete(address);
        await _uow.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result<AddressResponseDto>> GetAddressByIdAsync(int userId, int addressId)
    {
        var addresses = await _uow.Addresses.GetTableNoTracking().Where(a => a.Id == addressId && a.UserId == userId).ToListAsync();
        var address = addresses.FirstOrDefault();
        
        if (address is null)
            return Error.Validation("Address.NotFound", "Address not found");
            
        return Result<AddressResponseDto>.Ok(_mapper.Map<AddressResponseDto>(address));
    }

    public async Task<Result<IReadOnlyList<AddressResponseDto>>> GetAddressesAsync(int userId)
    {
        var addresses = await _uow.Addresses.GetTableNoTracking().Where(a => a.UserId == userId).ToListAsync();
        return Result<IReadOnlyList<AddressResponseDto>>.Ok(_mapper.Map<IReadOnlyList<AddressResponseDto>>(addresses));
    }

    public async Task<Result<UserProfileResponseDto>> GetProfileAsync(int userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return Error.Validation("User.NotFound", "User not found");
            
        return Result<UserProfileResponseDto>.Ok(_mapper.Map<UserProfileResponseDto>(user));
    }

    public async Task<Result<UserProfileResponseDto>> UpdateProfileAsync(int userId, UpdateProfileRequestDto request)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return Error.Validation("User.NotFound", "User not found");
            
        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.Name = $"{request.FirstName} {request.LastName}";
        user.Email = request.Email;
        user.PhoneNumber = request.PhoneNumber;
        
        await _userManager.UpdateAsync(user);
        
        return Result<UserProfileResponseDto>.Ok(_mapper.Map<UserProfileResponseDto>(user));
    }

    public async Task<Result> SetDefaultAddressAsync(int userId, SetDefaultAddressRequestDto request)
    {
        var addresses = await _uow.Addresses.FindAsync(a => a.UserId == userId);
        
        if (!addresses.Any())
            return Error.Validation("Address.NotFound", "No addresses found");
            
        foreach (var address in addresses)
        {
            address.IsDefault = (address.Id == request.AddressId);
        }
            
        await _uow.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result<AddressResponseDto>> UpdateAddressAsync(int userId, UpdateAddressRequestDto request)
    {
        var addresses = await _uow.Addresses.FindAsync(a => a.Id == request.AddressId && a.UserId == userId);
        var address = addresses.FirstOrDefault();
        
        if (address is null)
            return Error.Validation("Address.NotFound", "Address not found");

        if (request.IsDefault && !address.IsDefault)
        {
            var existingAddresses = await _uow.Addresses.FindAsync(a => a.UserId == userId && a.Id != request.AddressId);
            foreach (var existing in existingAddresses)
            {
                existing.IsDefault = false;
                _uow.Addresses.Update(existing);
            }
        }
            
        address.Label = request.Label;
        address.StreetAddress = request.StreetAddress;
        address.City = request.City;
        address.Governorate = request.Governorate;
        address.PhoneNumber = request.PhoneNumber;
        address.IsDefault = request.IsDefault;
        
        await _uow.SaveChangesAsync();
        
        return Result<AddressResponseDto>.Ok(_mapper.Map<AddressResponseDto>(address));
    }
}
