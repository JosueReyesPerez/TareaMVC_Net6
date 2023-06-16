using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using TareaMVC;
using TareaMVC.Services;

var builder = WebApplication.CreateBuilder(args);

//Creamos una politica de solo usuarios autenticados
var politicaUsuariosAutenticados = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();

// Add services to the container.
builder.Services.AddControllersWithViews(opt => {  //implementamos en el filtro las politicas
    opt.Filters.Add(new AuthorizeFilter(politicaUsuariosAutenticados));
}).AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix) //Sercivio de IView Localizer 
.AddDataAnnotationsLocalization(opt => {
    opt.DataAnnotationLocalizerProvider = (_, factoria) => factoria.Create(typeof(RecursoCompartido));
    });


// Add services context
builder.Services.AddDbContext<AplicationDbContext>(opt => opt.UseSqlServer("name=DefaultConnection"));

//Inyectamos el servicio de autenticacion 
builder.Services.AddAuthentication();

//Inyectamos la tablas donde se almacenaran los Usuarios y Roles
builder.Services.AddIdentity<IdentityUser, IdentityRole>(opt =>opt.SignIn.RequireConfirmedAccount = false).AddEntityFrameworkStores<AplicationDbContext>().AddDefaultTokenProviders();

builder.Services.PostConfigure<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme,
    opt =>
    {
        opt.LoginPath = "/usuarios/login";
        opt.AccessDeniedPath = "/usuario/login";
    });

builder.Services.AddLocalization(opt => { opt.ResourcesPath = "Recursos"; });

builder.Services.AddTransient<IServicioUsuarios, ServicioUsuarios>();

var app = builder.Build();

//Se inserta el servicio de culturas
app.UseRequestLocalization(opt => {
    opt.DefaultRequestCulture = new RequestCulture("es"); //Idioma por defecto
    opt.SupportedUICultures = Constantes.CulturasUISoportadas.Select(cultara => new CultureInfo(cultara.Value)).ToList();
    //Culturas soportadas, se pasa cada cultura a una instancia de CultureInfo
});

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
