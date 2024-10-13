using Expense_Tracker.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity; // Add this for Identity
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//DI
builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DevConnection")));

//Register Syncfusion license
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NDaF5cWWtCf1NpRGpGfV5ycEVHYlZUQHxcRU0DNHVRdkdnWH9fcnVXQ2RfVkV0X0U=");

// Add Identity services
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    // Configure password settings
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;

    // User settings.
    options.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = false; // Set to true if you want unique email
})
.AddEntityFrameworkStores<ApplicationDbContext>() // Use your DbContext for Identity
.AddDefaultTokenProviders();



// Configure logging
builder.Logging.ClearProviders(); // Optional: clear default providers if you want to customize
builder.Logging.AddConsole(); // Log to console
builder.Logging.AddDebug(); // Log to debug output

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

app.UseAuthentication();

app.UseAuthorization();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

// Custom middleware to restrict access to authenticated users
app.Use(async (context, next) =>
{
    // Check if the user is authenticated
    if (!context.User.Identity.IsAuthenticated &&
        !context.Request.Path.StartsWithSegments("/Account") && // Allow Account pages
        !context.Request.Path.StartsWithSegments("/Home") && // Allow Home page
        context.Request.Path != "/") // Allow root path
    {
        context.Response.Redirect("/Account/Login"); // Redirect to Login
        return;
    }
    await next(); // Proceed to the next middleware if authenticated
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


// Add routing specifically for the AccountController
app.MapControllerRoute(
    name: "account",
    pattern: "Account/{action}/{id?}",
    defaults: new { controller = "Account", action = "Register" });


app.Run();
