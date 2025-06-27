using Microsoft.EntityFrameworkCore;
using WebAppChamadosTI.Models;

namespace WebAppChamadosTI.Data
{
    public class BancoDados : DbContext
    {
        string conexao;

        //Mapeamento das tabelas do banco de dados
        public DbSet<Paciente> Pacientes { get; set; }
        public DbSet<Dentista> Dentistas { get; set; }
        public DbSet<Agendamento> Agendamentos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Agendamento>()
                .HasOne(b => b.Paciente)
                .WithMany(a => a.Agendamentos)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Agendamento>()
                .HasOne(b => b.Dentista)
                .WithMany(a => a.Agendamentos)
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
