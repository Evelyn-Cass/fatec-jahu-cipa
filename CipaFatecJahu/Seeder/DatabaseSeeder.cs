using System.Security.Claims;
using CipaFatecJahu.Models;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;

namespace CipaFatecJahu.Seeder
{
    public class DatabaseSeeder
    {
        private readonly ContextMongodb _context = new ContextMongodb();
        private readonly RoleManager<ApplicationRole> _roleManager;
        private UserManager<ApplicationUser> _userManager;

        public DatabaseSeeder(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task SeedAsync()
        {
            var existingMaterials = await _context.Materials.Find(u => true).ToListAsync();
            if (existingMaterials == null || !existingMaterials.Any())
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

                await _context.Materials.InsertManyAsync(initialMaterials);
            }

            if (!await _roleManager.RoleExistsAsync("Secretário"))
            {
                await _roleManager.CreateAsync(new ApplicationRole() { Name = "Secretário" });
            }

            if (!await _roleManager.RoleExistsAsync("Admnistrador"))
            {
                await _roleManager.CreateAsync(new ApplicationRole() { Name = "Administrador" });
            }

            if (await _userManager.FindByEmailAsync("adm@adm.com") == null)
            {

                ApplicationUser appuser = new ApplicationUser();
                appuser.Name = "Administrador";
                appuser.UserName = "adm@adm.com";
                appuser.Email = "adm@adm.com";
                appuser.Status = "Ativo";
                IdentityResult result = await _userManager.CreateAsync(appuser, "Administrador@1");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(appuser, "Administrador");
                    await _userManager.AddClaimAsync(appuser, new Claim("firstName", "Administrador"));
                }
            }
        }
    }
}
