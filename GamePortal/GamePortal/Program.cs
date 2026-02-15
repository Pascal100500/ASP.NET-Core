using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using GamePortal.Models;

var builder = WebApplication.CreateBuilder(args);

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
.AddEntityFrameworkStores<ApplicationDbContext>();

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

app.UseAuthentication(); 
app.UseAuthorization();

app.MapRazorPages();

app.Run();