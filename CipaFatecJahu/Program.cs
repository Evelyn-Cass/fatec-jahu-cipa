using CipaFatecJahu.Data;
using CipaFatecJahu.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<CipaFatecJahuContext>(options =>
  options.UseSqlServer(builder.Configuration.GetConnectionString("CipaFatecJahuContext") ?? throw new InvalidOperationException("Connection string 'CipaFatecJahuContext' not found.")));

// Add services to the container.  
builder.Services.AddControllersWithViews();



ContextMongodb.ConnectionString = builder.Configuration.GetSection("MongoConnection:ConnectionString").Value;
ContextMongodb.Database = builder.Configuration.GetSection("MongoConnection:Database").Value;
ContextMongodb.IsSSL = Convert.ToBoolean(builder.Configuration.GetSection("MongoConnection:IsSSL").Value);

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.User.RequireUniqueEmail = true;
})
.AddMongoDbStores<ApplicationUser, ApplicationRole, Guid>(
   ContextMongodb.ConnectionString, ContextMongodb.Database)
.AddDefaultTokenProviders().AddErrorDescriber<PortugueseIdentityErrorDescriber>();


var app = builder.Build();

// Configure the HTTP request pipeline.  
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
else
{
    app.UseDeveloperExceptionPage(); // Add this line to enable detailed error pages in development mode.
    var userManager = app.Services.CreateScope().ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = app.Services.CreateScope().ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
    DatabaseSeeder seeder = new DatabaseSeeder(userManager, roleManager);
    await seeder.SeedAsync();
}

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();


app.MapControllerRoute(
  name: "default",
  pattern: "{controller=Home}/{action=Index}/{id?}")
  .WithStaticAssets();


app.Run();
