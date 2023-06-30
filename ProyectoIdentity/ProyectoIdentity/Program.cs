using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProyectoIdentity.Data;

var builder = WebApplication.CreateBuilder(args);

//Cadena de conexi�n a SQL Server

builder.Services.AddDbContext<ApplicationDbContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("Conexi�nIdentitySql"))
);

//Agregar el servicio Identity a la aplicaci�n

builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();

//Url de retorno 

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = new PathString("/Cuentas/Acceso");
    options.AccessDeniedPath = new PathString("/Cuentas/Bloqueado");
});

//Configuraciones contrase�a m�s segura

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredLength = 8; //La contrase�a debe tener minimo 8 caracteres
    options.Password.RequireLowercase = true; //Requiere letras minusculas
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1); //Tiempo de bloqueo de un minuto, si no ingresa bien las credenciales
    options.Lockout.MaxFailedAccessAttempts = 3; //Intentos m�ximos para fallar la contrase�a 
});

// Add services to the container.
builder.Services.AddControllersWithViews();

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

//Agregar la Autenticaci�n

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
