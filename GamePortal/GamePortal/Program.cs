using GamePortal;
using GamePortal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
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

var dbProvider = builder.Configuration["DbProvider"];

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

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>() // ���������� ��� �������� ���� ��������������
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

// === �������� �������������� ����� ===
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

    // ������� ���� Admin ���� �� ���
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }

    // ��������� ���������� �� ������������
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
            Log.Information("Добавлен администратор {Email}", adminEmail); // ���������� � �������� ������ � ��������
        }
    }
}
app.MapGamesApi(); // Мой REST для игр
app.Run();
