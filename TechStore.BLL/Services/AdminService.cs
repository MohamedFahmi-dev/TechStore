using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TechStore.BLL.Interface;
using TechStore.DAL.Context;
using TechStore.DAL.Extensions;
using TechStore.DAL.UnitOfWork;
using TechStore.Domain.DTOs.Admin;
using TechStore.Domain.Entities;
using TechStore.Domain.Enums;

namespace TechStore.BLL.Services;

public class AdminService : IAdminService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _uow;

    public AdminService(UserManager<ApplicationUser> userManager, IMapper mapper, IUnitOfWork uow)
    {
        _userManager = userManager;
        _mapper = mapper;
        _uow = uow;
    }

    public async Task<Result<IReadOnlyList<UserResponseDto>>> GetAllUsersAsync()
    {
        var users = await _userManager.Users.ToListAsync();
        return Result<IReadOnlyList<UserResponseDto>>.Ok(_mapper.Map<IReadOnlyList<UserResponseDto>>(users));
    }

    public async Task<Result<UserResponseDto>> GetUserByIdAsync(int userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return Error.Validation("User.NotFound", "User not found");

        return Result<UserResponseDto>.Ok(_mapper.Map<UserResponseDto>(user));
    }

    public async Task<Result<UserResponseDto>> CreateUserAsync(CreateUserRequestDto request)
    {
        // Check if user already exists
        var existingUser = await _userManager.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email || 
                                     u.UserName == request.Username || 
                                     u.PhoneNumber == request.PhoneNumber);
        
        if (existingUser is not null)
            return Error.Validation("User.AlreadyExists", "User with this email, username, or phone number already exists.");
        
        var user = new ApplicationUser
        {
            UserName = request.Username,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Name = $"{request.FirstName} {request.LastName}",
            PhoneNumber = request.PhoneNumber,
        };

        await _uow.BeginTransactionAsync();

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            await _uow.RollbackTransactionAsync();
            return result.Errors.Select(e => Error.Validation(e.Code, e.Description)).ToList();
        }

        var roleResult = await _userManager.AddToRoleAsync(user, request.Role.ToString());
        if (!roleResult.Succeeded)
        {
            await _uow.RollbackTransactionAsync();
            return Error.Failure("RoleAssignmentFailed", "User created but role assignment failed");
        }

        await _uow.CommitTransactionAsync();
        return Result<UserResponseDto>.Ok(_mapper.Map<UserResponseDto>(user));
    }

    public async Task<Result<UserResponseDto>> UpdateUserAsync(UpdateUserRequestDto request)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user is null)
            return Error.Validation("User.NotFound", "User not found");

        user.UserName = request.Username;
        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.Name = $"{request.FirstName} {request.LastName}";
        user.Email = request.Email;
        user.PhoneNumber = request.PhoneNumber;
        user.AccountStatus = request.AccountStatus;

        await _userManager.UpdateAsync(user);
        return Result<UserResponseDto>.Ok(_mapper.Map<UserResponseDto>(user));
    }

    public async Task<Result> AssignRoleAsync(AssignRoleRequestDto request)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user is null)
            return Result.Fail(Error.Validation("User.NotFound", "User not found"));

        var result = await _userManager.AddToRoleAsync(user, request.Role.ToString());
        if (!result.Succeeded)
            return Result.Fail(result.Errors.Select(e => Error.Validation(e.Code, e.Description)).ToList());

        return Result.Ok();
    }

    public async Task<Result> DeactivateUserAsync(int userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return Result.Fail(Error.Validation("User.NotFound", "User not found"));

        user.AccountStatus = AccountStatus.Inactive;
        await _userManager.UpdateAsync(user);
        return Result.Ok();
    }

    public async Task<Result> ActivateUserAsync(int userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return Result.Fail(Error.Validation("User.NotFound", "User not found"));

        user.AccountStatus = AccountStatus.Active;
        await _userManager.UpdateAsync(user);
        return Result.Ok();
    }
}
