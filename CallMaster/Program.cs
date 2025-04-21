using CallMaster.Data;
using CallMaster.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using NuGet.Configuration;
using CallMaster.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var settings = new CallMaster.Models.Settings();
builder.Configuration.Bind("Settings", settings);
builder.Services.AddSingleton(settings);


builder.Services.AddDbContext<CallMasterDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Db"))
);

builder.Services.AddScoped<IAuthenticationServices, AuthenticationService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // Define the paths for login, logout, and access denied
        options.LoginPath = "/Account/Login";  // Path for login page
        options.LogoutPath = "/Account/Logout"; // Path for logout
        options.AccessDeniedPath = "/Account/AccessDenied"; // Path for access denied
        options.SlidingExpiration = true;  // Optional: set sliding expiration for cookies
        options.ExpireTimeSpan = TimeSpan.FromHours(168);  // Set cookie expiration time (1 hour here)
    });


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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
