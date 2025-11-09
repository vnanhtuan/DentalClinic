using DentalClinic.Application.Interfaces;
using DentalClinic.Application.Interfaces.Staffs;
using DentalClinic.Application.Interfaces.Roles;
using DentalClinic.Application.Services;
using DentalClinic.Application.Services.Staffs;
using DentalClinic.Application.Services.Roles;
using DentalClinic.Domain.Interfaces;
using DentalClinic.Infrastructure;
using DentalClinic.Infrastructure.Repositories;
using DentalClinic.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DentalClinicDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserManagementService, UserManagementService>();
builder.Services.AddScoped<IStaffService, StaffService>();
builder.Services.AddScoped<IRoleService, RoleService>();

builder.Services.AddMemoryCache();

// Security
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = new SymmetricSecurityKey(
    Encoding.UTF8.GetBytes(jwtSettings["Secret"]!)
);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false; // 'false' run local
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = secretKey,

        NameClaimType = ClaimTypes.Name, // Map "name" -> User.Identity.Name
        RoleClaimType = ClaimTypes.Role  // Map "http://.../role" -> User.IsInRole("Admin")
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

// ROUTE CHO MANAGE SPA (USING AREA)
app.MapControllerRoute(
    name: "manage",
    pattern: "manage/{*path}",
    defaults: new { area = "Manage", controller = "Manage", action = "Index" }
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
