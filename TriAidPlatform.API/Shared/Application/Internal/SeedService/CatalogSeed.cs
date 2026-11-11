using TriAidPlatform.API.Subscription.Domain.Model.Aggregates;
using TriAidPlatform.API.Subscription.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

namespace TriAidPlatform.API.Shared.Application.Internal.SeedService;

/// <summary>
///     Idempotent seed of the plan catalog. Runs at startup so a fresh
///     MySQL database always exposes the commercial plans.
/// </summary>
public static class CatalogSeed
{
    /// <summary>Inserts the plans when the catalog is empty.</summary>
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var plans = scope.ServiceProvider.GetRequiredService<PlanRepository>();
        if ((await plans.ListAsync()).Count > 0) return;

        foreach (var plan in DefaultPlans())
        {
            await plans.AddAsync(plan);
        }
        await plans.CompleteAsync();
    }

    private static IEnumerable<Plan> DefaultPlans()
    {
        yield return new Plan
        {
            Id = "plan-esencial", Name = "Esencial",
            For = "For small clinics and pilot projects",
            MonthlyPrice = 49, AnnualPrice = 470, MaxUsers = 5,
            Features = ["Up to 5 triage users", "Automatic vital signs capture",
                "Real-time out-of-range alerts", "Email support"]
        };
        yield return new Plan
        {
            Id = "plan-profesional", Name = "Professional",
            For = "For hospitals with multiple points of care",
            MonthlyPrice = 119, AnnualPrice = 1138, MaxUsers = 0,
            Features = ["Unlimited triage users", "Assisted NT-158 classification + analytics",
                "Multi-specialty routing", "Priority support"]
        };
        yield return new Plan
        {
            Id = "plan-institucional", Name = "Institutional",
            For = "For health networks and smart-city projects",
            MonthlyPrice = 249, AnnualPrice = 2390, MaxUsers = 100,
            Features = ["Multi-campus management", "API integration with hospital systems",
                "SLA + dedicated onboarding"]
        };
    }
}
