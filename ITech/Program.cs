using FastReport.Data;
using ITech.Areas.Admin.Services;
using ITech.Areas.Tecnicos.Services;
using ITech.Context;
using ITech.Models;
using ITech.Repositories;
using ITech.Repositories.Interfaces;
using ITech.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using ReflectionIT.Mvc.Paging;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        


        // Adiciona os serviços ao contêiner
        builder.Services.AddControllersWithViews();
        builder.Services.AddPaging(options =>
        {
            options.ViewName = "Bootstrap4";
            options.PageParameterName = "pageindex";
        });
        
        var connString = builder.Configuration.GetConnectionString("DefaultConnection");
        Console.WriteLine($"Connection string usada: {connString}");

        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connString));

        FastReport.Utils.RegisteredObjects.AddConnection(typeof(MsSqlDataConnection));

        builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                        .AddEntityFrameworkStores<AppDbContext>()
                        .AddDefaultTokenProviders();

        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Account/Login";
            options.AccessDeniedPath = "/Account/AccessDenied";
            options.ReturnUrlParameter = "returnUrl";
        });

        builder.Services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 6;
            options.Password.RequiredUniqueChars = 1;
        });

        builder.Services.Configure<ConfigurationImagens>(
        builder.Configuration.GetSection("ConfigurationImagens"));


        // Registro de repositórios e serviços
        builder.Services.AddTransient<IServicoRepository, ServicoRepository>();
        builder.Services.AddTransient<ICategoriaRepository, CategoriaRepository>();
        builder.Services.AddTransient<IPedidoRepository, PedidoRepository>();

        builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("Admin", politica =>
            {
                politica.RequireRole("Admin");
            });
        });

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("Tecnicos", politica =>
            {
                politica.RequireRole("Tecnicos");
            });
        });

        builder.Services.AddScoped<ISeedUserRoleInitial, SeedUserRoleInitial>();
        builder.Services.AddScoped<AdminRelatorioVendasService>();
        builder.Services.AddScoped<AdminGraficoVendasService>();
        builder.Services.AddScoped<AdminRelatorioServicosService>();
        builder.Services.AddScoped<TecnicosRelatorioVendasService>();
        builder.Services.AddScoped<TecnicosGraficoVendasService>();
        builder.Services.AddScoped<TecnicosRelatorioServicosService>(); 
        builder.Services.AddScoped<ITecnicoRepository, TecnicoRepository>();

        builder.Services.AddScoped(sp => CarrinhoCompra.GetCarrinho(sp));

        builder.Services.AddMemoryCache();
        builder.Services.AddSession();

        

        var app = builder.Build();

        // Seed de roles e usuário (executado na inicialização)
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var seedUserRoleInitial = services.GetRequiredService<ISeedUserRoleInitial>();
            seedUserRoleInitial.SeedRoles();
            seedUserRoleInitial.SeedUser();

            //var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            //db.Database.Migrate();
        }

        // Configuração do pipeline HTTP
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseFastReport();

        app.UseRouting();

        app.UseSession();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "admin_area",
                pattern: "Admin/{controller=Home}/{action=Index}/{id?}",
                defaults: new { area = "Admin" });

            endpoints.MapControllerRoute(
                name: "tecnicos_area",
                pattern: "Tecnicos/{controller=Home}/{action=Index}/{id?}",
                defaults: new { area = "Tecnicos" });

            endpoints.MapControllerRoute(
               name: "categoriaFiltro",
                pattern: "Servico/{action}/{categoria?}",
                defaults: new { Controller = "Servico", Action = "List" });

            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        });

        app.MapControllerRoute(
            name: "login_redirect_fix",
            pattern: "Admin/Account/Login",
            defaults: new { controller = "Account", action = "Login", area = "" }
        );

        app.Run();
    }
}
