using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Store.API.Middleware;
using Store.API.Services;
using Store.Core.Interfaces;
using Store.Core.Interfaces.Repositories;
using Store.Core.Interfaces.Services;
using Store.Repository.Data;
using Store.Repository.Repositories;
using Store.Repository.UnitOfWork;
using Store.Service.Services;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// ==================================================
// 1 — Database Connection
// ==================================================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// ==================================================
// 2 — Register Repositories
// ==================================================
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductCategoryRepository, ProductCategoryRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IShippingRepository, ShippingRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

// ==================================================
// 3 — Register Unit of Work
// ==================================================
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// ==================================================
// 4 — Register Services
// ==================================================
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IProductCategoryService, ProductCategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IShippingService, ShippingService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IImageUploadService, ImageUploadService>();

// ==================================================
// 5 — Register Auth Services
// ==================================================
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// ==================================================
// 6 — JWT Authentication
// ==================================================
var key = builder.Configuration["JwtSettings:Key"]!;

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.MapInboundClaims = false;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"❌ Auth Failed: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine("✅ Token Validated!");
            return Task.CompletedTask;
        }
    };
});

// ==================================================
// 7 — Authorization Policies
// ==================================================
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireClaim("role", "Admin"));
    options.AddPolicy("CustomerOnly", policy => policy.RequireClaim("role", "Customer"));
    options.AddPolicy("AllUsers", policy => policy.RequireClaim("role", "Admin", "Customer"));
});

// ==================================================
// 8 — CORS
// ==================================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("OnlineStorePolicy", policy =>
    {
        policy
            .WithOrigins(
                "https://localhost:7267",
                "http://localhost:5222"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});



// ==================================================
// 9 — Rate Limiting Policies
// ==================================================
builder.Services.AddRateLimiter(options =>
{
    // --------------------------------------------------
    // Auth Policy — 5 requests per minute
    // For: POST /api/auth/login + POST /api/auth/register
    // Protects against brute force attacks
    // --------------------------------------------------
    options.AddFixedWindowLimiter("AuthPolicy", limiterOptions =>
    {
        limiterOptions.PermitLimit = 5;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 0;
    });

    // --------------------------------------------------
    // Read Policy — 100 requests per minute
    // For: All GET endpoints
    // --------------------------------------------------
    options.AddFixedWindowLimiter("ReadPolicy", limiterOptions =>
    {
        limiterOptions.PermitLimit = 100;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 0;
    });

    // --------------------------------------------------
    // Write Policy — 30 requests per minute
    // For: All POST, PUT, DELETE endpoints
    // --------------------------------------------------
    options.AddFixedWindowLimiter("WritePolicy", limiterOptions =>
    {
        limiterOptions.PermitLimit = 30;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 0;
    });

    // --------------------------------------------------
    // Custom response when limit exceeded
    // --------------------------------------------------
    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = 429;
        context.HttpContext.Response.ContentType = "application/json";

        // Tell client how long to wait
        var retryAfter = context.Lease.TryGetMetadata(
            MetadataName.RetryAfter, out var retryAfterValue)
            ? (int)retryAfterValue.TotalSeconds
            : 60;

        context.HttpContext.Response.Headers["Retry-After"] = retryAfter.ToString();

        var response = new
        {
            success = false,
            message = $"Too many requests. Please try again in {retryAfter} seconds.",
            data = (object?)null,
            retryAfter = retryAfter
        };

        await context.HttpContext.Response.WriteAsJsonAsync(response, cancellationToken);
    };
});


// ==================================================
// 10 — Controllers with Validation
// ==================================================
builder.Services.AddControllers()
       .AddJsonOptions(options =>
       {
           options.JsonSerializerOptions.ReferenceHandler =
               System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
       })
       .ConfigureApiBehaviorOptions(options =>
       {
           //  Return validation errors in ApiResponse format
           options.InvalidModelStateResponseFactory = context =>
           {
               var errors = context.ModelState
                   .Where(e => e.Value?.Errors.Count > 0)
                   .SelectMany(e => e.Value!.Errors)
                   .Select(e => e.ErrorMessage)
                   .ToList();

               var response = new
               {
                   success = false,
                   message = string.Join(", ", errors),
                   data = (object?)null
               };

               return new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(response);
           };
       });

builder.Services.AddEndpointsApiExplorer();

// ==================================================
// 11 — Swagger with JWT
// ==================================================
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "Online Store API",
        Version = "v1",
        Description = "API for managing an online store"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your token here}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// ==================================================
// 12 — Middleware Pipeline
// ==================================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Online Store API v1");
        options.RoutePrefix = "swagger";
    });
}

//  Global error handling — must be first
app.UseGlobalErrorHandling();
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseCors("OnlineStorePolicy");
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
