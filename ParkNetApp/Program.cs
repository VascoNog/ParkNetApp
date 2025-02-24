using Microsoft.AspNetCore.Identity;
using Microsoft.Build.Framework;
using System.Globalization;

public class Program
{
    public static async Task Main(string[] args)
    {
        var cultureInfo = new CultureInfo("pt-PT");
        CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
        CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        var connectionString = builder.Configuration.GetConnectionString("ParkNetConnection") ?? throw new InvalidOperationException("Connection string 'ParkNetConnection' not found.");
        builder.Services.AddDbContext<ParkNetDbContext>(options =>
            options.UseSqlServer(connectionString));
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ParkNetDbContext>();
        builder.Services.AddRazorPages();

        builder.Services.AddScoped<ParkNetRepository>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization();

        app.MapStaticAssets();
        app.MapRazorPages()
           .WithStaticAssets();

        using (var scope = app.Services.CreateScope())
        {
            var roleManager = scope.ServiceProvider
                .GetRequiredService<RoleManager<IdentityRole>>();

            var roles = new[] { "Admin", "Manager", "Member" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        using (var scope = app.Services.CreateScope())
        {
            var userManager = scope.ServiceProvider
                .GetRequiredService<UserManager<IdentityUser>>();

            string email = "admin@admin.com";
            string password = "Test1234!";

            if (await userManager.FindByEmailAsync(email) == null)
            {
                var user = new IdentityUser();
                user.UserName = email;
                user.Email = email;
                user.EmailConfirmed = true;

                await userManager.CreateAsync(user, password);
                await userManager.AddToRoleAsync(user, "Admin");
            }

            // Garante que todos os utilizadores sem role são "Member"
            var users = userManager.Users.ToList();
            foreach (var user in users)
            {
                var roles = await userManager.GetRolesAsync(user);
                if (!roles.Any()) // Se o utilizador não tiver nenhum role, atribuir "Member"
                {
                    await userManager.AddToRoleAsync(user, "Member");
                }
            }
        }

        app.Run();
    }
}