using TriAidPlatform.API.Shared.Domain.Model;
using TriAidPlatform.API.Shared.Domain.Model.Errors;

namespace TriAidPlatform.API.Iam.Domain.Model.Errors;

public static class IamErrors
{
    public static readonly Error InvalidCredentials =
        Error.Unauthorized("Iam.InvalidCredentials", "Invalid username or password.");

    public static readonly Error UsernameAlreadyTaken =
        Error.Conflict("Iam.UsernameAlreadyTaken", "The specified username is already taken.");

    public static readonly Error InstitutionAlreadyRegistered =
        Error.Conflict("Iam.InstitutionAlreadyRegistered", "The specified institution is already registered.");

    public static readonly Error UserNotFound =
        Error.NotFound("Iam.UserNotFound", "The specified user was not found.");

    public static readonly Error NotEnoughPermissions =
        Error.Forbidden("Iam.NotEnoughPermissions", "You do not have permission to perform this operation.");

    public static readonly Error SeatLimitReached =
        Error.Forbidden("Iam.SeatLimitReached", "The plan seat limit for this institution has been reached.");

    public static readonly Error UserCreationFailed =
        Error.Failure("Iam.UserCreationFailed", "An error occurred while creating the user.");
}
