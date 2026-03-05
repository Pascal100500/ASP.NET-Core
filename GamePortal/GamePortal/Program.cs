using GamePortal;
using GamePortal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Context;
using Serilog.Events;

// �������� ����� 
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    // ����� � ��� ���� ������ �������� ������ ������ ���������� � ������ ������
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Error) // ���������� �������� ������� ����������� �� ������ 
    .Enrich.FromLogContext()  // для разделения для кого выводить лог, user или админ
    .WriteTo.Console()

    // User log
    .WriteTo.Logger(lc => lc
    .Filter.ByIncludingOnly(e =>
        e.Properties.ContainsKey("LogType") &&
        e.Properties["LogType"].ToString().Contains("User"))
    .WriteTo.File(
        "Logs/user.log",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}")
    )
    // Admin Log
    .WriteTo.Logger(lc => lc
    .Filter.ByIncludingOnly(e =>
        e.Properties.ContainsKey("LogType") &&
        e.Properties["LogType"].ToString().Contains("Admin"))
    .WriteTo.File(
    "Logs/admin.log",
    rollingInterval: RollingInterval.Day,
    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}") //������� [{SourceContext}], ����� ������ �������� ���������
    // ������� Level �� Level:u3 (�������� ������� (INF, WRN, ERR)
    // ������� Message Message:lj (��������� ������� ����������������� ���������)
    )
    .CreateLogger();
    

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

// ВЫБЕРИТЕ ПРОВАЙДЕРА В ФАЙЛЕ appsettings.json
// Доступно: SqlServer, SQlite, Postgres

//  !!!  WARNING  !!!
// По задумке база данных SQLite должна находиться в папке Data в корне проекта.

var dbProvider = builder.Configuration["DbProvider"];

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    switch (dbProvider)
    {
        case "Postgres":
            options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres"));
            break;

        case "SQLite":
            options.UseSqlite(builder.Configuration.GetConnectionString("SQLite"),
                 b => b.MigrationsAssembly("GamePortal"));
            break;

        default:
            options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
            break;
    }
});

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // отключение подтвержения эмеил адреса
    options.Password.RequireDigit = false; // Цифры в пароле не обязательны
    options.Password.RequireLowercase = false; // не обязательное наличие строчных букв
    options.Password.RequireUppercase = false; // не обязательное наличие заглавных букв
    options.Password.RequireNonAlphanumeric = false; // не обязательное наличие специальных символов
    options.Password.RequiredLength = 6; // оставил длину пароля 6 символов минимум
})
/*
 Когда вызывается await _userManager.CreateAsync(user, Input.Password);
НАХОДИТСЯ В ФАЙЛЕ "Register.cshtml.cs"

Identity:
- Проверяет валидаторы пароля
- Если пароль не проходит — возвращает список ошибок
- Эти ошибки попадают в ModelState
 */


.AddRoles<IdentityRole>() 
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddErrorDescriber<RussianIdentityErrorDescriber>();

builder.Services.AddRazorPages();
//Swagger
//  https://localhost:7093/swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

#region Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
#endregion

// Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); 
app.UseAuthorization();

app.MapRazorPages();

// === Создание администратора и категорий для игр при первом запуске, а так же папки Data и всего прочего вспомогательного ===
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
    var db = services.GetRequiredService<ApplicationDbContext>();

    try
    {
        bool databaseExists = db.Database.CanConnect(); // Добавил метод для проверки может ли EF подключиться к базе данных, чтобы узнать создана она или еще нет
        if (!databaseExists)
        { 
        
            // Если Работаем с SQLite то я создаю папку Data.
            if (dbProvider == "SQLite")
            {
                // Создаю папку Data для SQLite
                var dataDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Data");
                Directory.CreateDirectory(dataDirectory);
                using (LogContext.PushProperty("LogType", "Admin"))
                {
                    Log.Information("Создана папка для хранения базы данных SQLite"); // Лог о создании папки Data для SQLite
                }

            }
            // Если Работаем с PostgreSQL или SQLite
            if (dbProvider == "SQLite" || dbProvider == "Postgres")
            {

                db.Database.EnsureCreated();
                
                using (LogContext.PushProperty("LogType", "Admin"))
                {
                    Log.Information("{Provider} База данных создана через EnsureCreated()", dbProvider);
                 }
                 
            }

             // Если работаем с SQL Server. К сожалению, сделать универсальные миграции и для PostgreSQL не удалось(
            else
            {
                 db.Database.Migrate(); // УЧИТЫВАЮ ВСЕ МИГРАЦИИ ПРИ СОЗДАНИИ БД
                
                using (LogContext.PushProperty("LogType", "Admin"))
                {
                    Log.Information("{Provider}База данных создана через миграции", dbProvider);
                }
                
            }
        }

        // Admin 
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }

        var adminEmail = "admin@example.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            var newAdmin = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(newAdmin, "Admin123!");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(newAdmin, "Admin");
                using (LogContext.PushProperty("LogType", "User"))
                {
                    Log.Information("Добавлен администратор {Email}", adminEmail); // Исправил лог о создании администратора
                }
            }
        }
    

        // Категории игр
        if (!db.Categories.Any())
        {
            db.Categories.AddRange(
                new Category { Name = "Action" },
                new Category { Name = "RPG" },
                new Category { Name = "Strategy" },
                new Category { Name = "Adventure" },
                new Category { Name = "Simulation" }
            );

            db.SaveChanges();
        }
    }
    catch (Exception ex) 
    {
        using (LogContext.PushProperty("LogType", "Admin"))
        {
            Log.Error(ex, "Ошибка при инициализации базы данных");
        }
        throw;
    }
   
}
app.MapGamesApi(); // Мой REST для игр
app.Run();
