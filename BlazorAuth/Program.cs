using BlazorAuth.Auth;
using BlazorAuth.Components;
using BlazorAuth.Data;
using BlazorAuth.Endpoints.Account;
using BlazorAuth.Models;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient();

builder.Services.AddAntiforgery();
builder.Services.AddHttpContextAccessor();
//builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

builder.Services.AddResponseCompression(opt =>
    opt.EnableForHttps = true);

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
        .UseSeeding(async (context, _) => await DatabaseInitialiser.SeedRole(context)));

builder.Services.AddIdentity<User, IdentityRole<Guid>>(opt =>
{
    opt.Password.RequireDigit = true;
    opt.Password.RequiredLength = 6;
    opt.Password.RequireNonAlphanumeric = false;

    opt.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    opt.User.RequireUniqueEmail = false;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(opt =>
    {
        opt.Cookie.Name = "SetCookie";
        opt.Cookie.HttpOnly = true;
        opt.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        opt.Cookie.SameSite = SameSiteMode.Lax;
        opt.SlidingExpiration = true;
        opt.ExpireTimeSpan = TimeSpan.FromDays(7);
        opt.LoginPath = "/Account/Login";
        opt.LogoutPath = "/Account/Logout";
        opt.AccessDeniedPath = "/Account/AccessDenied";

        opt.Events.OnRedirectToLogin = (context) =>
        {
            context.Response.StatusCode = 401;
            return Task.CompletedTask;
        };

        opt.Events = new CookieAuthenticationEvents
        {
            OnValidatePrincipal = (context) =>
            {   
                if(context.Principal?.Identity?.IsAuthenticated ?? false)
                {
                    var expirationClaim = context.Principal?.FindFirst(ClaimTypes.Expiration)?.Value;
                    if(DateTimeOffset.TryParse(expirationClaim, out var expiration) && expiration < DateTimeOffset.UtcNow)
                    {
                        context.RejectPrincipal();
                        return context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    }
                }
                else
                {
                    context.RejectPrincipal();
                }

                return Task.CompletedTask;
            }
        };
    });


builder.Services.AddBlazoredSessionStorage();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

// - Auth
app.UseAuthentication();
app.UseAuthorization();
app.UseResponseCaching();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// - Map Endpoints
app.MapRegisterEndpoint();

app.Run();
