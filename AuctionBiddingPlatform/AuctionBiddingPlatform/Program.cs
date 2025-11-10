using AuctionBiddingPlatform.Application.Mappings;
using AuctionBiddingPlatform.Application.Services;
using AuctionBiddingPlatform.Core.Entities;
using AuctionBiddingPlatform.Core.Interfaces.IRepositories;
using AuctionBiddingPlatform.Core.Interfaces.IServices;
using AuctionBiddingPlatform.Core.Interfaces.Messaging;
using AuctionBiddingPlatform.Infrastructure.Data;
using AuctionBiddingPlatform.Infrastructure.Messaging;
using AuctionBiddingPlatform.Infrastructure.Repositories;
using AuctionBiddingPlatform.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1️⃣ Database Context
builder.Services.AddDbContext<AuctionDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// 2️⃣ ASP.NET Identity Configuration
builder.Services.AddIdentityCore<ApplicationUser>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddRoles<ApplicationRole>()
.AddEntityFrameworkStores<AuctionDbContext>()
.AddSignInManager<SignInManager<ApplicationUser>>();

// 3️⃣ JWT Authentication Setup
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey missing.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

// 🔹 Required for [Authorize] attributes and policies
builder.Services.AddAuthorization();

// 4️⃣ AutoMapper
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<MappingProfile>();
});

// 5️⃣ Dependency Injection for Repositories & UnitOfWork
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped<IAuctionItemRepository, AuctionItemRepository>();
builder.Services.AddScoped<IBidRepository, BidRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// 6️⃣ Application Services
builder.Services.AddScoped<IAuctionItemService, AuctionItemService>();
builder.Services.AddScoped<IBidService, BidService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAuctionQueryService, AuctionQueryService>();
builder.Services.AddScoped<IBidValidationService, BidValidationService>();



// Messaging
builder.Services.AddSingleton<IMessagePublisher, RabbitMqPublisher>();


// 7️⃣ Controllers & Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Auction Bidding Platform API",
        Version = "v1",
        Description = "Secure backend for online auction bidding system"
    });

    // 🔹 Add JWT authentication schema
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Enter 'Bearer' [space] and then your valid JWT token.\n\nExample: Bearer eyJhbGciOiJIUzI1NiIs...",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // 🔹 Apply authentication globally
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});
var app = builder.Build();

// 8️⃣ HTTP Pipeline Configuration
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 🔹 Global Exception Handler
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

// 🔹 Authentication & Authorization must be in this order
app.UseAuthentication();

app.UseAuthorization();

// 🔹 Custom Middleware for Bid Validation (runs after user is authenticated)
app.UseMiddleware<BiddingRulesMiddleware>();

app.MapControllers();

app.Run();