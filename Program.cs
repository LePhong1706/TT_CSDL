using Microsoft.EntityFrameworkCore;
using Web_vuottai.Data; // namespace chứa AppDbContext
using QuestPDF.Infrastructure;
using QuestPDF.Drawing;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var fontsPath = Path.Combine(builder.Environment.WebRootPath, "fonts");
FontManager.RegisterFont(File.OpenRead(Path.Combine(fontsPath, "NotoSans-Regular.ttf")));
FontManager.RegisterFont(File.OpenRead(Path.Combine(fontsPath, "NotoSans-Bold.ttf")));

QuestPDF.Settings.License = LicenseType.Community;

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
