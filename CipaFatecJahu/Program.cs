using System.Globalization;
using CipaFatecJahu.Data;
using CipaFatecJahu.Models;
using CipaFatecJahu.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

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
.AddDefaultTokenProviders();


builder.Services.AddSingleton<MongoDbService>();



var app = builder.Build();

// Configure the HTTP request pipeline.  
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
else
{
    app.UseDeveloperExceptionPage(); // Add this line to enable detailed error pages in development mode.  
}

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
  name: "default",
  pattern: "{controller=Home}/{action=Index}/{id?}")
  .WithStaticAssets();

using (var scope = app.Services.CreateScope())
{
    var mongoService = scope.ServiceProvider.GetRequiredService<MongoDbService>();
    var materialCollection = mongoService.Material;

    var existingMaterials = materialCollection.Find(_ => true).Any();
    if (!existingMaterials)
    {
        var initialMaterials = new List<Material>
           {
           new Material { Id = new Guid("9b927360-b531-4bb9-9e09-1a3093f8507a"), Description = "ATAS" },
           new Material { Id = new Guid("d2f6b9f0-3b1a-4e4e-9b8e-1c3d2a4f7c8b"), Description = "Curso CIPA" },
           new Material { Id = new Guid("7e4c8f2d-9b4e-4b8d-8b7e-2c3d3a5f6d9c"), Description = "Documentos" },
           new Material { Id = new Guid("3b8d3f2e-1a1c-4e4e-9b8e-1c3d2a4f7c8b"), Description = "Eleição" },
           new Material { Id = new Guid("2a7d3f2e-9b4e-4b8d-8b7e-2c3d3a5f6d9c"), Description = "Estudos" },
           new Material { Id = new Guid("1a6d3f2e-8b4e-4b8d-8b7e-2c3d3a5f6d9c"), Description = "Legislação" },
           new Material { Id = new Guid("0a5d3f2e-7b4e-4b8d-8b7e-2c3d3a5f6d9c"), Description = "Mapas de Risco" },
           new Material { Id = new Guid("8c5d3f2e-5b4e-4b8d-8b7e-2c3d3a5f6d9c"), Description = "Membros" },
           new Material { Id = new Guid("9a4d3f2e-6b4e-4b8d-8b7e-2c3d3a5f6d9c"), Description = "SIPAT" }
           };

        materialCollection.InsertMany(initialMaterials);
    }
}

app.Run();
