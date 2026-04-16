using Eshop.API.Extensions;
using Eshop.Application.DependencyInjection;
using Eshop.Infrastructure.Data;
using Eshop.Infrastructure.DependencyInjection;
using Eshop.Infrastructure.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Serilog;
using System.Reflection.Metadata;

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build())
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) =>
    {
        configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext();
    });

    // Add services to the container.

    builder.Services.AddControllers();
    // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
    //builder.Services.AddOpenApi();

    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddHttpContextAccessor();
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);

    var jwtOptions = builder.Configuration
        .GetSection(JwtOptions.SectionName)
        .Get<JwtOptions>() ?? throw new InvalidOperationException("Jwt settings are missing.");

    builder.Services.Configure<JwtOptions>(
        builder.Configuration.GetSection("Jwt"));

    Console.WriteLine("Environment: " + builder.Environment.EnvironmentName);
    Console.WriteLine("Jwt:SecretKey => " + builder.Configuration["Jwt:SecretKey"]);


    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidAudience = jwtOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
                ClockSkew = TimeSpan.Zero
            };
        });

    builder.Services.AddAuthorization();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Eshop API",
            Version = "v1",
        });

        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "JWT Authorization header using the Bearer scheme. Example: {your token}"
        });

        options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        });
    });
    var app = builder.Build();

    app.UseRequestLoggingMiddleware();
    app.UseCustomExceptionMiddleware();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        //app.MapOpenApi();
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly.");
}
finally
{
    Log.CloseAndFlush();
}