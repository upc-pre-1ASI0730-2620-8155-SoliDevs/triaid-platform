using Microsoft.EntityFrameworkCore;
using TriAidPlatform.API.PatientRegistration.Application.Internal;
using TriAidPlatform.API.PatientRegistration.Domain.Repositories;
using TriAidPlatform.API.PatientRegistration.Domain.Services;
using TriAidPlatform.API.PatientRegistration.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using TriAidPlatform.API.Shared.Domain.Repositories;
using TriAidPlatform.API.TriageClassification.Application.Internal.CommandServices;
using TriAidPlatform.API.TriageClassification.Application.Internal.QueryServices;
using TriAidPlatform.API.TriageClassification.Domain.Repositories;
using TriAidPlatform.API.TriageClassification.Domain.Services;
using TriAidPlatform.API.TriageClassification.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using TriAidPlatform.API.TriageClassification.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using TriAidPlatform.API.SpecialtyAssignment.Application.Internal.CommandServices;
using TriAidPlatform.API.SpecialtyAssignment.Domain.Repositories;
using TriAidPlatform.API.SpecialtyAssignment.Domain.Services;
using TriAidPlatform.API.SpecialtyAssignment.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using TriAidPlatform.API.SpecialtyAssignment.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using TriAidPlatform.API.Shared.Infrastructure.Interfaces.Asp.Configuration;
using TriAidPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using TriAidPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySQL(builder.Configuration.GetConnectionString("DefaultConnection")!));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Patient Registration bounded context
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IPatientRegistrationService, PatientRegistrationService>();
builder.Services.AddScoped<ITriageEpisodeRepository, TriageEpisodeRepository>();
builder.Services.AddScoped<ITriageEpisodeService, TriageEpisodeService>();

// Triage Classification bounded context
builder.Services.AddScoped<IClassificationRepository, ClassificationRepository>();
builder.Services.AddScoped<IClassificationService, ClassificationCommandService>();
builder.Services.AddScoped<ClassificationQueryService>();

// Specialty Assignment bounded context
builder.Services.AddScoped<ISpecialtyRepository, SpecialtyRepository>();
builder.Services.AddScoped<IReferralRepository, ReferralRepository>();
builder.Services.AddScoped<ISpecialtyAssignmentService, SpecialtyAssignmentService>();

builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers(options=> options.Conventions.Add(new KebabCaseRouteNamingConvention()));
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Create the database schema if it does not exist yet
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
    SpecialtyCatalogSeed.SeedSpecialtyCatalog(context);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();