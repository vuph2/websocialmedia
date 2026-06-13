using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using web.Data;
using web.Hubs;
using web.Models;
using web.Services;

var builder = WebApplication.CreateBuilder(args);

// Set DataDirectory for LocalDB AttachDbFilename
var dataDirectory = Path.Combine(builder.Environment.ContentRootPath, "Databases");
AppDomain.CurrentDomain.SetData("DataDirectory", dataDirectory);

// ── Upload Limits ───────────────────────────────────────────────
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 104857600; // 100MB
});

builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 104857600; // 100MB
});

// ── Database ──────────────────────────────────────────────────
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (connectionString != null && connectionString.Contains("|DataDirectory|"))
{
    connectionString = connectionString.Replace("|DataDirectory|", dataDirectory);
}
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        connectionString,
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null
        )
    )
);

// ── Identity ──────────────────────────────────────────────────
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// ── Google OAuth 2.0 ──────────────────────────────────────────
var googleClientId = builder.Configuration["Authentication:Google:ClientId"] ?? "";
var googleClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? "";
var googleEnabled = !string.IsNullOrEmpty(googleClientId)
                 && googleClientId != "YOUR_GOOGLE_CLIENT_ID"
                 && !string.IsNullOrEmpty(googleClientSecret)
                 && googleClientSecret != "YOUR_GOOGLE_CLIENT_SECRET";

if (googleEnabled)
{
    builder.Services.AddAuthentication()
        .AddGoogle(options =>
        {
            options.ClientId = googleClientId;
            options.ClientSecret = googleClientSecret;
            options.Scope.Add("profile");
        });
}

// Store the flag so controllers/views can check it
builder.Services.AddSingleton(new GoogleAuthSettings { Enabled = googleEnabled });

// ── Cookie ────────────────────────────────────────────────────
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath         = "/Account/Login";
    options.LogoutPath        = "/Account/Logout";
    options.AccessDeniedPath  = "/Account/Login";
    options.ExpireTimeSpan    = TimeSpan.FromDays(7);
    options.SlidingExpiration = true;
});

// ── SignalR ───────────────────────────────────────────────────
builder.Services.AddSignalR();

builder.Services.AddControllersWithViews();

// ── Background Services ───────────────────────────────────────
builder.Services.AddHostedService<StoryCleanupService>();

var app = builder.Build();

// ── Auto-Migration & Role Seeding (Development only) ─────────────
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();

    // Seed Roles
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    string[] roles = ["Admin", "Moderator", "User"];
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }

    // Seed default Admin account
    var userManager2 = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    const string adminEmail = "admin@nexus.com";
    var adminUser = await userManager2.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        var admin = new ApplicationUser
        {
            UserName = "admin",
            Email = adminEmail,
            FirstName = "Admin",
            LastName = "Nexus",
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow
        };
        var result = await userManager2.CreateAsync(admin, "Admin@123456");
        if (result.Succeeded)
            await userManager2.AddToRoleAsync(admin, "Admin");
    }
    else if (!await userManager2.IsInRoleAsync(adminUser, "Admin"))
    {
        await userManager2.AddToRoleAsync(adminUser, "Admin");
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

// ── SignalR Hub ───────────────────────────────────────────────
app.MapHub<NexusHub>("/nexusHub");

app.Run();

/// <summary>
/// Simple settings class to share Google OAuth status with controllers/views.
/// </summary>
public class GoogleAuthSettings
{
    public bool Enabled { get; set; }
}
