using Microsoft.EntityFrameworkCore;
using Store.Core.Interfaces;
using Store.Core.Interfaces.Repositories;
using Store.Core.Interfaces.Services;
using Store.Repository.Data;
using Store.Repository.Repositories;
using Store.Repository.UnitOfWork;
using Store.Service.Services;

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

// ==================================================
// 5 — Controllers and Swagger
// ==================================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "Online Store API",
        Version = "v1",
        Description = "API for managing an online store — customers, products, orders, payments, shippings, and reviews"
    });
});

var app = builder.Build();

// ==================================================
// 6 — Middleware Pipeline
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

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
