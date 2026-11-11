using TriAidPlatform.API.Subscription.Domain.Model.Aggregates;

namespace TriAidPlatform.API.Subscription.Domain.Services;

/// <summary>
/// Payment gateway port. The simulated implementation approves every
/// charge unless the masked card ends with '0000' (demo rejection).
/// </summary>
public interface IPaymentGateway
{
    /// <summary>Charges an amount against a stored card.</summary>
    GatewayResult Charge(decimal amount, string method);
}

/// <summary>Outcome of a gateway charge attempt.</summary>
/// <param name="Approved">Whether the charge went through.</param>
/// <param name="Status">'paid' or 'rejected'.</param>
/// <param name="Method">Masked card used.</param>
public record GatewayResult(bool Approved, string Status, string Method);
