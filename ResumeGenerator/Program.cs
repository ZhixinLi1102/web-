using QuestPDF.Infrastructure;
using QuestPDF;
using ResumeGenerator.Services;

var builder = WebApplication.CreateBuilder(args);

Settings.License = LicenseType.Community;

// 1. 添加 MVC 服务
builder.Services.AddControllersWithViews();

// 2. 添加 Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddScoped<IPdfService, PdfService>();
builder.Services.AddHttpContextAccessor();
var app = builder.Build();

// 3. 启用静态文件
app.UseStaticFiles();

// 4. 启用 Session
app.UseSession();

// 5. 启用路由和 MVC
app.UseRouting();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Resume}/{action=Index}/{id?}");

app.Run();