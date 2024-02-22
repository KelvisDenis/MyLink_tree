using Microsoft.EntityFrameworkCore;
using MySite.Data;
using MySite.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<MySiteContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString(
    "MySiteContext") ?? throw new InvalidOperationException("Connection string 'Site_1Context' not found."
    )));
builder.Services.AddScoped<UserServices>();



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
    pattern: "{controller=User}/{action=Login}/{id?}");

app.Run();
