using System.Diagnostics;
using System.Security.Claims;
using Core.Entities;
using DataBaseApi.Persistence;
using DotNetEd.CoreAdmin;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING") ?? throw new InvalidOperationException("CONNECTION_STRING environment variable is not set.");

builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseNpgsql(connectionString));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/access-denied";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });
builder.Services.AddAuthorization();

builder.Services.AddCoreAdmin(new CoreAdminOptions
{
    RestrictToRoles = ["Admin"],
    IgnoreEntityTypes = [typeof(WeekDay)]
});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var adminUsername = Environment.GetEnvironmentVariable("ADMIN_USERNAME") ?? throw new InvalidOperationException("ADMIN_USERNAME environment variable is not set.");
var adminPassword = Environment.GetEnvironmentVariable("ADMIN_PASSWORD") ?? throw new InvalidOperationException("ADMIN_PASSWORD environment variable is not set.");

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/login", async (HttpContext context) =>
{
    context.Response.ContentType = "text/html";
    await context.Response.WriteAsync(@"
        <html>
        <head>
            <title>Login</title>
            <style>
                body {
                    font-family: Arial, sans-serif;
                    background-color: #f0f2f5;
                    display: flex;
                    justify-content: center;
                    align-items: center;
                    height: 100vh;
                    margin: 0;
                }
                .login-container {
                    background-color: #fff;
                    padding: 30px;
                    border-radius: 10px;
                    box-shadow: 0 0 10px rgba(0,0,0,0.1);
                    width: 300px;
                }
                h2 {
                    margin-bottom: 20px;
                    text-align: center;
                }
                label {
                    display: block;
                    margin-bottom: 10px;
                }
                input[type='text'], input[type='password'] {
                    width: 100%;
                    padding: 8px;
                    margin-top: 5px;
                    margin-bottom: 15px;
                    border: 1px solid #ccc;
                    border-radius: 4px;
                }
                button {
                    width: 100%;
                    padding: 10px;
                    background-color: #007bff;
                    color: white;
                    border: none;
                    border-radius: 4px;
                    cursor: pointer;
                }
                button:hover {
                    background-color: #0056b3;
                }
            </style>
        </head>
        <body>
            <div class='login-container'>
                <h2>Login to CoreAdmin</h2>
                <form method='post' action='/login'>
                    <label>Username:
                        <input type='text' name='username' required />
                    </label>
                    <label>Password:
                        <input type='password' name='password' required />
                    </label>
                    <button type='submit'>Login</button>
                </form>
            </div>
        </body>
        </html>
    ");
});


app.MapPost("/login", async (HttpContext context) =>
{
    var username = context.Request.Form["username"];
    var password = context.Request.Form["password"];
    app.Logger.LogInformation($"Login attempt for username: {username}");

    if (username == adminUsername && password == adminPassword)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, "Admin")
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(2)
        });

        app.Logger.LogInformation($"User {username} logged in successfully");
        context.Response.Redirect("/coreadmin");
        return;
    }

    app.Logger.LogWarning($"Failed login attempt for username: {username}");
    await Task.Delay(2000);
    context.Response.Redirect("/access-denied");
});

app.MapGet("/access-denied", async (HttpContext context) =>
{
    context.Response.ContentType = "text/html";
    await context.Response.WriteAsync(@"
        <html>
        <head>
            <title>Access Denied</title>
            <style>
                body {
                    font-family: Arial, sans-serif;
                    background-color: #fff3f3;
                    color: #a94442;
                    display: flex;
                    justify-content: center;
                    align-items: center;
                    height: 100vh;
                    margin: 0;
                }
                .denied-container {
                    text-align: center;
                    background-color: #f2dede;
                    padding: 40px;
                    border-radius: 10px;
                    box-shadow: 0 0 10px rgba(0,0,0,0.1);
                    max-width: 400px;
                }
                h2 {
                    margin-bottom: 15px;
                }
                p {
                    margin-bottom: 20px;
                }
                a.button {
                    display: inline-block;
                    padding: 10px 20px;
                    background-color: #007bff;
                    color: white;
                    text-decoration: none;
                    border-radius: 4px;
                }
                a.button:hover {
                    background-color: #0056b3;
                }
            </style>
        </head>
        <body>
            <div class='denied-container'>
                <h2>Access Denied</h2>
                <p>You do not have permission to access this page.</p>
                <a class='button' href='/login'>Try Again</a>
            </div>
        </body>
        </html>
    ");
});

app.MapControllers();
app.MapRazorPages();

app.MapDefaultControllerRoute();

app.Run();