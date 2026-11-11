using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using TriAidPlatform.API.Iam.Application.Acl;
using TriAidPlatform.API.Iam.Application.CommandServices;
using TriAidPlatform.API.Iam.Application.Internal.CommandServices;
using TriAidPlatform.API.Iam.Application.Internal.OutboundServices;
using TriAidPlatform.API.Iam.Application.Internal.QueryServices;
using TriAidPlatform.API.Iam.Application.QueryServices;
using TriAidPlatform.API.Iam.Domain.Repositories;
using TriAidPlatform.API.Iam.Infrastructure.Hashing.BCrypt.Services;
using TriAidPlatform.API.Iam.Infrastructure.Pipeline.Middleware.Extensions;
using TriAidPlatform.API.Iam.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using TriAidPlatform.API.Iam.Infrastructure.Tokens.Jwt.Configuration;
using TriAidPlatform.API.Iam.Infrastructure.Tokens.Jwt.Services;
using TriAidPlatform.API.Iam.Interfaces.Acl;
using TriAidPlatform.API.PatientRegistration.Application.Internal;
using TriAidPlatform.API.PatientRegistration.Domain.Repositories;
using TriAidPlatform.API.PatientRegistration.Domain.Services;
using TriAidPlatform.API.PatientRegistration.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using TriAidPlatform.API.Shared.Application.Internal.SeedService;
using TriAidPlatform.API.Alerting.Application.Internal.CommandServices;
using TriAidPlatform.API.Alerting.Application.Internal.QueryServices;
using TriAidPlatform.API.Alerting.Domain.Repositories;
using TriAidPlatform.API.Alerting.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using TriAidPlatform.API.Shared.Domain.Repositories;
using TriAidPlatform.API.Shared.Infrastructure.Interfaces.Asp.Configuration;
using TriAidPlatform.API.Shared.Infrastructure.Interfaces.Asp.Configuration.OpenApi;
using TriAidPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using TriAidPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using TriAidPlatform.API.SpecialtyAssignment.Application.Internal.CommandServices;
using TriAidPlatform.API.SpecialtyAssignment.Domain.Repositories;
using TriAidPlatform.API.SpecialtyAssignment.Domain.Services;
using TriAidPlatform.API.SpecialtyAssignment.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using TriAidPlatform.API.SpecialtyAssignment.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using TriAidPlatform.API.Subscription.Application.Internal.CommandServices;
using TriAidPlatform.API.Subscription.Application.Internal.QueryServices;
using TriAidPlatform.API.Subscription.Domain.Repositories;
using TriAidPlatform.API.Subscription.Domain.Services;
using TriAidPlatform.API.Subscription.Infrastructure.Gateway.Simulated;
using TriAidPlatform.API.Subscription.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using TriAidPlatform.API.TriageClassification.Application.Internal.CommandServices;
using TriAidPlatform.API.TriageClassification.Application.Internal.QueryServices;
using TriAidPlatform.API.TriageClassification.Domain.Repositories;
using TriAidPlatform.API.TriageClassification.Domain.Services;
using TriAidPlatform.API.TriageClassification.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using TriAidPlatform.API.TriageClassification.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Database connection
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySQL(builder.Configuration.GetConnectionString("DefaultConnection")!));

// Shared bounded context
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IInstitutionRepository, InstitutionRepository>();

// Patient Registration bounded context
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IPatientRegistrationService, PatientRegistrationService>();
builder.Services.AddScoped<ITriageEpisodeRepository, TriageEpisodeRepository>();
builder.Services.AddScoped<ITriageEpisodeService, TriageEpisodeService>();

// Alerting bounded context
builder.Services.AddScoped<IAlertRepository, AlertRepository>();
builder.Services.AddScoped<AlertCommandService>();
builder.Services.AddScoped<AlertQueryService>();

// IAM bounded context
builder.Services.Configure<TokenSettings>(builder.Configuration.GetSection("TokenSettings"));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IHashingService, HashingService>();
builder.Services.AddSingleton<ITokenService, TokenService>();
builder.Services.AddScoped<IUserCommandService, UserCommandService>();
builder.Services.AddScoped<IUserQueryService, UserQueryService>();
builder.Services.AddScoped<IIamContextFacade, IamContextFacade>();

// Subscription bounded context
builder.Services.AddScoped<PlanRepository>();
builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
builder.Services.AddSingleton<IPaymentGateway, SimulatedPaymentGateway>();
builder.Services.AddScoped<ISubscriptionCommandService, SubscriptionCommandService>();
builder.Services.AddScoped<ISubscriptionQueryService, SubscriptionQueryService>();
builder.Services.AddScoped<SubscriptionQueryService>();

// Triage Classification bounded context
builder.Services.AddScoped<IClassificationRepository, ClassificationRepository>();
builder.Services.AddScoped<IClassificationService, ClassificationCommandService>();
builder.Services.AddScoped<ClassificationQueryService>();

// Specialty Assignment bounded context
builder.Services.AddScoped<ISpecialtyRepository, SpecialtyRepository>();
builder.Services.AddScoped<IReferralRepository, ReferralRepository>();
builder.Services.AddScoped<ISpecialtyAssignmentService, SpecialtyAssignmentService>();

builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers(options =>
    options.Conventions.Add(new KebabCaseRouteNamingConvention()));

// OpenAPI / Scalar documentation (no Swagger UI)
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllPolicy",
        policy => policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

// Catalog seed (idempotent, runs after the host is ready)
await CatalogSeed.SeedAsync(app.Services);

app.UseHttpsRedirection();
app.UseCors();

// IAM request authorization: resolves the Bearer token into the user
app.UseRequestAuthorization();

app.UseAuthorization();

app.MapControllers();

app.Run();
