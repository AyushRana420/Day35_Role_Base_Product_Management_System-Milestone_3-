using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Role_Base_Product_Management_System.Data;
using Role_Base_Product_Management_System.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Connection String
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//Identity & Password Policy
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;//Special Characters is required 
    options.Password.RequireDigit = true;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

//Configuring Application Cookie
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

//Adding Data Protection for encryption of sensitive data
builder.Services.AddDataProtection();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

//Seed roles & default users
await SeedRolesAndUsers(app.Services);

app.Run();

//Creating a local function to seed roles & admin users
async Task SeedRolesAndUsers(IServiceProvider serviceProvider)
{
    using var scope = serviceProvider.CreateScope();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    string[] roles = { "Admin", "Manager" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }

    //Create admin user
    var admin = await userManager.FindByNameAsync("admin");
    if (admin == null)
    {
        admin = new ApplicationUser { UserName = "admin", Email = "admin@local", EmailConfirmed = true };
        await userManager.CreateAsync(admin, "Admin@123"); // meets policy above
        await userManager.AddToRoleAsync(admin, "Admin");
    }

    // Seed manager user
    var manager = await userManager.FindByNameAsync("manager");
    if (manager == null)
    {
        manager = new ApplicationUser { UserName = "manager", Email = "manager@local", EmailConfirmed = true };
        await userManager.CreateAsync(manager, "Manager@123"); // meets policy above
        await userManager.AddToRoleAsync(manager, "Manager");
    }
}
