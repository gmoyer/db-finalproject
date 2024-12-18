using Microsoft.EntityFrameworkCore;
using ServerSubscriptionManager.Context;
using ServerSubscriptionManager.Filters;
using ServerSubscriptionManager.Models;
using ServerSubscriptionManager.Services;
using System.Security.Claims;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
    .AddControllers(options =>
        {
            options.Filters.Add<TransactionWrapper>();
        })
    .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        });


builder.Services.AddDbContext<SubscriptionContext>();

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<IEntityService<Payment>, PaymentService>();
builder.Services.AddScoped<IEntityService<Invoice>, InvoiceService>();
builder.Services.AddScoped<IEntityService<SubscriptionPeriod>, SubscriptionPeriodService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication("Cookies")
    .AddCookie(options =>
    {
        options.Cookie.Name = "AuthCookie";
        options.Cookie.HttpOnly = true; // Prevents JavaScript access to the cookie
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Cookie is sent only over HTTPS
        options.Cookie.SameSite = SameSiteMode.None; // Allows the cookie to be sent with cross-origin requests
        options.LoginPath = "/api/auth/login";
        options.LogoutPath = "/api/auth/logout";
        options.Cookie.IsEssential = true; // Ensures cookie isn't removed if tracking is disabled
    });
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    {
        builder.WithOrigins("http://localhost:4200") // Allowed origin for Angular app
               .AllowCredentials() // Allows sending cookies
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("Admin", policy => policy.RequireRole("Admin"))
    .AddPolicy("User", policy => policy.RequireRole("User"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseCors("CorsPolicy");

app.MapControllers();

app.Run();
