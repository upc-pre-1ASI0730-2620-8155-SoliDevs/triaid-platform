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
using TriAidPlatform.API.Shared.Domain.Repositories;
using TriAidPlatform.API.Shared.Infrastructure.Interfaces.Asp.Configuration;
using TriAidPlatform.API.Shared.Infrastructure.Interfaces.Asp.Configuration.OpenApi;
using TriAidPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using TriAidPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Database connection
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySQL(builder.Configuration.GetConnectionString("DefaultConnection")!));

// Shared bounded context
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// IAM bounded context
builder.Services.Configure<TokenSettings>(builder.Configuration.GetSection("TokenSettings"));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IHashingService, HashingService>();
builder.Services.AddSingleton<ITokenService, TokenService>();
builder.Services.AddScoped<IUserCommandService, UserCommandService>();
builder.Services.AddScoped<IUserQueryService, UserQueryService>();
builder.Services.AddScoped<IIamContextFacade, IamContextFacade>();

builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers(options =>
    options.Conventions.Add(new KebabCaseRouteNamingConvention()));

// OpenAPI / Scalar documentation (no Swagger UI)
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

// IAM request authorization: resolves the Bearer token into the user
app.UseRequestAuthorization();

app.UseAuthorization();

app.MapControllers();

app.Run();
