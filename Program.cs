using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using WebApplication2.Data;
using WebApplication2.Data.Repositories; // 🔥 adiciona este
using WebApplication2.Services;          // 🔥 adiciona este

var builder = WebApplication.CreateBuilder(args);

// Banco de Dados
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Serviços
builder.Services.AddControllersWithViews();
builder.Services.AddControllers(); // para API Controllers
builder.Services.AddSession();

// 🔥 Registar Repositórios e Serviços
builder.Services.AddScoped<IUtilizadorRepository, UtilizadorRepository>();
builder.Services.AddScoped<IUtilizadorService, UtilizadorService>();

// 🔥 ADICIONAR Autenticação por Cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Home/Login"; // página de login
        options.LogoutPath = "/Home/Logout"; // página de logout
        options.AccessDeniedPath = "/Home/AcessoNegado"; // (se quiseres uma página de acesso negado)
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
    });

builder.Services.AddAuthorization();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Sandbox API",
        Version = "v1",
        Description = "API para a aplicação Blazor Sandbox"
    });
});

var app = builder.Build();

// Middlewares
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sandbox API V1");
    });
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

// 🔥 Estes têm de vir nesta ordem:
app.UseAuthentication();
app.UseAuthorization();

// Rotas
app.MapControllers(); // APIs
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"); // MVC tradicional

app.Run();
