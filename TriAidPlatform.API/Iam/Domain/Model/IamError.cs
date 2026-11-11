namespace TriAidPlatform.API.Iam.Domain.Model;

public enum IamError
{
    None,
    UserNotFound,
    UsernameAlreadyTaken,
    InstitutionAlreadyRegistered,
    InvalidCredentials,
    NotEnoughPermissions,
    SeatLimitReached,
    OperationCancelled,
    DatabaseError,
    InternalServerError,
    ExternalServiceError
}
