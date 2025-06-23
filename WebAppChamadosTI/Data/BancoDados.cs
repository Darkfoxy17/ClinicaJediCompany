using Microsoft.EntityFrameworkCore;
using WebAppChamadosTI.Models;

namespace WebAppChamadosTI.Data
{
    public class BancoDados : DbContext
    {
        string conexao;

        //Mapeamento das tabelas do banco de dados
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Tecnico> Tecnicos { get; set; }
        public DbSet<Chamado> Chamados { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Chamado>()
                .HasOne(b => b.Cliente)
                .WithMany(a => a.Chamados)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Chamado>()
                .HasOne(b => b.Tecnico)
                .WithMany(a => a.Chamados)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            conexao = @"Server=localhost\SQLExpress;
Database=ChamadosTIBd;
Integrated Security=True;
TrustServerCertificate=True";
            optionsBuilder.UseSqlServer(conexao);
            base.OnConfiguring(optionsBuilder);
        }
        public DbSet<WebAppChamadosTI.Models.ContaViewModel> ContaViewModel { get; set; } = default!;
        public DbSet<WebAppChamadosTI.Models.RegistroViewModel> RegistroViewModel { get; set; } = default!;

    }
}
