using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class Startup
{
    // Autres configurations...

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseSession(); // Ajout de la gestion des sessions

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        });

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "reservation",
                pattern: "{controller=Reservation}/{action=ConfirmReservation}/{id?}");
            // D'autres routes peuvent être définies ici si nécessaire
        });

        app.UseRouting();

        app.UseAuthorization();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddControllersWithViews();

        services.Configure<CookiePolicyOptions>(options =>
        {
            options.MinimumSameSitePolicy = SameSiteMode.None;
            options.HttpOnly = HttpOnlyPolicy.Always;
            options.Secure = CookieSecurePolicy.Always;
            options.OnAppendCookie = cookieContext => { };
            options.OnDeleteCookie = cookieContext => { };
        });

        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.None;
                options.Events = new CookieAuthenticationEvents
                {
                    OnValidatePrincipal = context =>
                    {
                        // Ajouter des en-têtes anti-cache ici
                        context.Response.Headers["Cache-Control"] = "no-cache, no-store";
                        context.Response.Headers["Pragma"] = "no-cache";
                        context.Response.Headers["Expires"] = "-1";

                        return Task.CompletedTask;
                    }
                };
            });

        services.AddSession(); // Ajout de la gestion des sessions
    }

}