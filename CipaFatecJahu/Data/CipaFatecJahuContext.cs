using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CipaFatecJahu.Models;

namespace CipaFatecJahu.Data
{
    public class CipaFatecJahuContext : DbContext
    {
        public CipaFatecJahuContext (DbContextOptions<CipaFatecJahuContext> options)
            : base(options)
        {
        }

        public DbSet<CipaFatecJahu.Models.Document> Document { get; set; } = default!;
        public DbSet<CipaFatecJahu.Models.Mandate> Mandate { get; set; } = default!;
    }
}
