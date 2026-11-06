using TriAidPlatform.API.Subscription.Domain.Model.Aggregates;
using TriAidPlatform.API.Subscription.Domain.Services;

namespace TriAidPlatform.API.Subscription.Infrastructure.Gateway.Simulated;

/// <summary>
/// Simulated gateway for development: approves every charge unless the
/// masked card ends with '0000' (demo rejection path).
/// </summary>
public class SimulatedPaymentGateway : IPaymentGateway
{
    /// <inheritdoc />
    public GatewayResult Charge(decimal amount, string method)
    {
        var normalized = method.Replace(" ", "");
        var rejected = normalized.EndsWith("0000");
        return new GatewayResult(!rejected, rejected ? "rejected" : "paid", method);
    }
}
