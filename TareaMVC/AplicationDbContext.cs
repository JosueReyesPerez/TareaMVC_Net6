using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TareaMVC.Entities;
using TareaMVC.Models;

namespace TareaMVC
{
    public class AplicationDbContext : IdentityDbContext
    {
        public AplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        //Models
        public DbSet<Post> Posts { get; set; }
        public DbSet<Blog> Blogs { get; set; }

        //Entities
        public DbSet<Tarea> Tareas { get; set; }
        public DbSet<Paso> Pasos { get; set; }
        public DbSet<ArchivoAdjunto> ArchivosAdjuntos { get; set; }
    }
}
