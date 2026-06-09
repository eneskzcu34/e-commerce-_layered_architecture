using Application.Managers;
using E_Shopping.Application.DependencyInjection;
using E_Shopping.Infrastructure.DependencyInjection;
using Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Identity;
using WebUI.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews();
builder.Services.AddSqlServer<AppDbContext>(builder.Configuration.GetConnectionString("MyDbConnections"));
builder.Services.AddMiniProfiler(options =>
{
    options.RouteBasePath = "/profiler";
    options.ShouldProfile = request =>
   {
       return !request.Path.Value.Contains("/images")
           && !request.Path.Value.Contains("/css")
           && !request.Path.Value.Contains("/js");
   };
}).AddEntityFramework();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Errors/AccessDenied";
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
    options.SlidingExpiration = true;
    options.Cookie.Name = "MyApp.Auth";
});
builder.Services.AddApplication();
builder.Services.AddInfrastructure();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.UseMiniProfiler();
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

SeedData.Initialize(app);
app.Run();
