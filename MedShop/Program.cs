using MedShop.Extensions;
using MedShop.Infrastructure.Data;
using MedShop.Core.Data.Models;
using MedShop.ModelBinders;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Relaxed password policy — this is a demo/portfolio project, not a production store.
builder.Services.AddDefaultIdentity<User>(options =>
    {
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireDigit = false;
        options.Password.RequireUppercase = false;
        options.Password.RequiredLength = 4;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
});
builder.Services.AddControllersWithViews(options =>
{
    // Apply CSRF token validation globally so every state-changing POST is protected by default.
    options.Filters.Add<AutoValidateAntiforgeryTokenAttribute>();
    // Replace the default decimal binder so that forms submitted with either '.' or ',' as the
    // decimal separator are parsed correctly regardless of the server's current culture.
    options.ModelBinderProviders.Insert(0, new DecimalModelBinderProvider());
});
builder.Services.AddMedShopServices();
builder.Services.AddDistributedMemoryCache();

// Override default Identity redirect paths so unauthenticated users are routed through
// the custom login/logout actions instead of the Razor Pages Identity scaffold.
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/User/Login";
    options.LogoutPath = "/User/Logout";
});

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");

    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
// Session must be initialised before authentication so the shopping cart session ID is
// available when Identity resolves the current user.
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    // Area routes must be registered before the default route so that Admin area
    // controllers are matched first.
    endpoints.MapControllerRoute(
      name: "areas",
      pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    );
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}"
    );
    endpoints.MapControllerRoute(
        name: "productDetails",
        pattern: "Product/Details/{id}/{information}"
    );
    endpoints.MapControllerRoute(
        name: "productEdit",
        pattern: "Product/Edit/{id}/{information}"
    );
    endpoints.MapControllerRoute(
        name: "productDelete",
        pattern: "Product/Delete/{id}/{information}"
    );

    endpoints.MapRazorPages();
});

// Ensure the Administrator role and seed user exist before the application begins handling
// requests.  Runs once at startup and is a no-op when already seeded.
await app.SeedAdminAsync();

app.Run();
