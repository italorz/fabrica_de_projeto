using Microsoft.EntityFrameworkCore;
using WebApplication1.Entities;

namespace WebApplication1.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Condutor> Condutores { get; set; }
        public DbSet<Veiculo> Veiculos { get; set; }
        public DbSet<TipoMulta> TiposMulta { get; set; }
        public DbSet<Multa> Multas { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=multas.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurações de relacionamentos
            modelBuilder.Entity<Multa>()
                .HasOne(m => m.Veiculo)
                .WithMany(v => v.Multas)
                .HasForeignKey(m => m.VeiculoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Multa>()
                .HasOne(m => m.Usuario)
                .WithMany(u => u.Multas)
                .HasForeignKey(m => m.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Multa>()
                .HasOne(m => m.Condutor)
                .WithMany(c => c.Multas)
                .HasForeignKey(m => m.CondutorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Multa>()
                .HasOne(m => m.TipoMulta)
                .WithMany(t => t.Multas)
                .HasForeignKey(m => m.TipoMultaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configurações de enums
            modelBuilder.Entity<Usuario>()
                .Property(u => u.Tipo)
                .HasConversion<string>();

            modelBuilder.Entity<TipoMulta>()
                .Property(t => t.Gravidade)
                .HasConversion<string>();

            // Configuração de precisão decimal para SQLite
            modelBuilder.Entity<TipoMulta>()
                .Property(t => t.ValorMulta)
                .HasPrecision(10, 2);
        }
    }
}