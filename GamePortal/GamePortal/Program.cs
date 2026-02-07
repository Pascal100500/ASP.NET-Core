using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using GamePortal.Models;

var builder = WebApplication.CreateBuilder(args);

// 1️⃣ Определяем провайдер
var dbProvider = builder.Configuration["DbProvider"];

// 2️⃣ Регистрируем DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    switch (dbProvider)
    {
        case "Postgres":
            options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres"));
            break;

        case "SQLite":
            options.UseSqlite(builder.Configuration.GetConnectionString("SQLite"));
            break;

        default:
            options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
            break;
    }
});

// 3️⃣ Подключаем Identity
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>();

// 4️⃣ Razor Pages
builder.Services.AddRazorPages();

var app = builder.Build();

// Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // 👈 обязательно
app.UseAuthorization();

app.MapRazorPages();

app.Run();