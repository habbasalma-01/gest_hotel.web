using gest_hotel.web.Models;
using gest_hotel.web.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace gest_hotel.web.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Ajoutez ici les DbSet pour vos entités
        public DbSet<Client> Clients { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Chambre> Chambres { get; set; }
        public DbSet<Reservation> Reservations { get; set; } // Ajoutez la DbSet pour la classe Reservation


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Chambre>()
                .Property(c => c.PrixParNuit)
                .HasColumnType("decimal(18,2)"); // Définir la précision et l'échelle appropriées

            // Autres configurations du modèle ici
        }
    }
}
