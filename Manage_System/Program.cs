using AspNetCoreHero.ToastNotification;
using Manage_System.Hubs;
using Manage_System.models;
using Manage_System.Models;
using Manage_System.Service;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddSession((option) =>
{
    option.IdleTimeout = new TimeSpan(0,30,0);
/*    option.Cookie.HttpOnly = true;
    option.Cookie.IsEssential = true;*/
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ManageSystem1640Context>(options =>
    options.UseSqlServer(connectionString));

    
builder.Services.AddAuthentication("AccountId")
    .AddCookie("AccountId",p =>
    {
        p.LoginPath = "/Login";
        p.LogoutPath = "/Logout";
        p.AccessDeniedPath = "/AccessDenied";
        p.Cookie.Name = "AccountId";
    });

builder.Services.AddAuthorization(p =>
{
    p.AddPolicy("Admin",
        policy => policy.RequireClaim("Admin", "Admin"));
    p.AddPolicy("Guest",
        policy => policy.RequireClaim("Guest", "Guest"));
    p.AddPolicy("Coordinator",
        policy => policy.RequireClaim("Coordinator", "Coordinator"));
    p.AddPolicy("Maketting",
        policy => policy.RequireClaim("Maketting", "Maketting"));
    p.AddPolicy("Student",
        policy => policy.RequireClaim("Student", "Student"));
});


builder.Services.AddSignalR();
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddNotyf(config => { config.DurationInSeconds = 5; config.IsDismissable = true; config.Position = NotyfPosition.TopRight; });

builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();


builder.Services.AddTransient<IFileService, FileService>();
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddTransient<IChatService, ChatService>();

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

app.UseSession();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
      name: "areas",
      pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    );
});

app.MapHub<ChatHub>("/chatHub");



app.Run();
