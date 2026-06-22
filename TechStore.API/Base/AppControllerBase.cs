using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TechStore.DAL.Extensions;
using TechStore.Domain.Enums;

namespace TechStore.API.Base;

[ApiController]
public abstract class AppControllerBase : ControllerBase
{
    protected int CurrentUserId 
    {
        get 
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(idClaim, out var id) || id <= 0)
                throw new UnauthorizedAccessException("User is not authenticated or token is invalid.");
            return id;
        }
    }

    protected ActionResult Success(object? value = null)
        => Ok(value);

    protected ActionResult CreatedSuccess(object? value = null)
        => StatusCode(StatusCodes.Status201Created, value);

    protected ActionResult UnauthorizedResult(string message = "Unauthorized")
        => Unauthorized(new { message });


    protected ActionResult Handle(Result result)
    {
        if (result.IsSuccess)
            return Success();

        return Problem(result.Errors);
    }

    protected ActionResult Handle<T>(Result<T> result, bool created = false)
    {
        if (result.IsSuccess)
            return created ? CreatedSuccess(result.Value) : Success(result.Value);

        return Problem(result.Errors);
    }


    private ActionResult Problem(IReadOnlyList<Error> errors)
    {
        var statusCode = MapErrorTypeToStatusCode(errors[0].Type);
        var body = new
        {
            errors = errors.Select(e => new { e.Code, e.Description })
        };
        return StatusCode(statusCode, body);
    }

    private static int MapErrorTypeToStatusCode(ErrorType errorType) => errorType switch
    {
        ErrorType.NotFound => StatusCodes.Status404NotFound,
        ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
        ErrorType.Forbidden => StatusCodes.Status403Forbidden,
        ErrorType.Validation => StatusCodes.Status400BadRequest,
        ErrorType.InvalidCredentials => StatusCodes.Status401Unauthorized,
        ErrorType.Failure => StatusCodes.Status500InternalServerError,
        _ => StatusCodes.Status500InternalServerError
    };
}
