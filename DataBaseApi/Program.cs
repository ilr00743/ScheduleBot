using System.Security.Claims;
using DataBaseApi.Persistence;
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

builder.Services.AddCoreAdmin("Admin");

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
        <body>
            <h2>Login to CoreAdmin</h2>
            <form method='post' action='/login'>
                <label>Username: <input type='text' name='username' required /></label><br/>
                <label>Password: <input type='password' name='password' required /></label><br/>
                <button type='submit'>Login</button>
            </form>
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
    context.Response.Redirect("/login?error=InvalidCredentials");
});

app.MapGet("/access-denied", async (HttpContext context) =>
{
    context.Response.ContentType = "text/html";
    await context.Response.WriteAsync("<h2>Access Denied</h2><p>You do not have permission to access this page.</p>");
});

app.MapControllers();
app.MapRazorPages();

app.MapDefaultControllerRoute();

app.Run();