using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Vortex.API;
using Vortex.Domain.Entities;
using Vortex.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.UTF8.GetBytes(jwtSettings["AuthenticationSecretKey"]!);

// Add services to the container.
builder.Services.AddDependencyInjection(builder.Configuration);
builder.Services.AddControllers();

#region CORS Policy

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "VortexPolicy", policyBuilder =>
    {
        policyBuilder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

#endregion

#region Add Identity
builder.Services.AddIdentity<UserEntity, IdentityRole<Guid>>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
})
.AddEntityFrameworkStores<VortexDbContext>()
.AddDefaultTokenProviders();;
#endregion

#region Add Auth
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero,
            NameClaimType = ClaimTypes.NameIdentifier,
        };
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AuthorizationPolicy", policy => policy.RequireAuthenticatedUser());
#endregion

# region Add Swagger Config
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Vortex API", Version = "v1" });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter Bearer token **_only_**",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, Array.Empty<string>() }
    });
});
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseCors("VortexPolicy");
app.UseAuthorization();

app.MapControllers();

app.Run();