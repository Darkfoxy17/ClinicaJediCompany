using Microsoft.EntityFrameworkCore;
using WebAppChamadosTI.Models;

namespace WebAppChamadosTI.Data
{
    public class BancoDados : DbContext
    {
        string conexao;

        // DbSets principais
        public DbSet<Paciente> Pacientes { get; set; }
        public DbSet<Dentista> Dentistas { get; set; }
        public DbSet<Agendamento> Agendamentos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Procedimento> Procedimentos { get; set; }
        public DbSet<StatusAgendamento> StatusAgendamentos { get; set; }
        public DbSet<DentistaProcedimento> DentistaProcedimentos { get; set; }

        // Novos DbSets
        public DbSet<Especializacao> Especializacoes { get; set; }
        public DbSet<EspecializacaoProcedimento> EspecializacoesProcedimentos { get; set; }

        // ViewModels (não persistidos)
        public DbSet<WebAppChamadosTI.Models.ContaViewModel> ContaViewModel { get; set; } = default!;
        public DbSet<WebAppChamadosTI.Models.RegistroViewModel> RegistroViewModel { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Relacionamento: Agendamento x Paciente
            modelBuilder.Entity<Agendamento>()
                .HasOne(a => a.Paciente)
                .WithMany(p => p.Agendamentos)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            // Relacionamento: Agendamento x Dentista
            modelBuilder.Entity<Agendamento>()
                .HasOne(a => a.Dentista)
                .WithMany(d => d.Agendamentos)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            // Relacionamento muitos-para-muitos: Dentista x Procedimento
            modelBuilder.Entity<DentistaProcedimento>()
                .HasKey(dp => new { dp.DentistaId, dp.ProcedimentoId });

            modelBuilder.Entity<DentistaProcedimento>()
                .HasOne(dp => dp.Dentista)
                .WithMany(d => d.DentistaProcedimentos)
                .HasForeignKey(dp => dp.DentistaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DentistaProcedimento>()
                .HasOne(dp => dp.Procedimento)
                .WithMany(p => p.DentistaProcedimentos)
                .HasForeignKey(dp => dp.ProcedimentoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relacionamento muitos-para-muitos: Especializacao x Procedimento
            modelBuilder.Entity<EspecializacaoProcedimento>()
                .HasKey(ep => new { ep.EspecializacaoId, ep.ProcedimentoId });

            modelBuilder.Entity<EspecializacaoProcedimento>()
                .HasOne(ep => ep.Especializacao)
                .WithMany(e => e.EspecializacaoProcedimentos)
                .HasForeignKey(ep => ep.EspecializacaoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EspecializacaoProcedimento>()
                .HasOne(ep => ep.Procedimento)
                .WithMany(p => p.EspecializacaoProcedimentos)
                .HasForeignKey(ep => ep.ProcedimentoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed de Especializações
            modelBuilder.Entity<Especializacao>().HasData(
                new Especializacao { Id = 1, Nome = "Clínico Geral" },
                new Especializacao { Id = 2, Nome = "Endodontia" },
                new Especializacao { Id = 3, Nome = "Ortodontia" },
                new Especializacao { Id = 4, Nome = "Implantodontia" },
                new Especializacao { Id = 5, Nome = "Odontopediatria" },
                new Especializacao { Id = 6, Nome = "Periodontia" }
            );

            // Seed de Procedimentos
            modelBuilder.Entity<Procedimento>().HasData(
                new Procedimento { Id = 1, Nome = "Avaliação" },
                new Procedimento { Id = 2, Nome = "Limpeza" },
                new Procedimento { Id = 3, Nome = "Canal" },
                new Procedimento { Id = 4, Nome = "Extração" },
                new Procedimento { Id = 5, Nome = "Clareamento" },
                new Procedimento { Id = 6, Nome = "Aparelho Ortodôntico" },
                new Procedimento { Id = 7, Nome = "Manutenção de Aparelho" },
                new Procedimento { Id = 8, Nome = "Implante Dentário" },
                new Procedimento { Id = 9, Nome = "Prótese" },
                new Procedimento { Id = 10, Nome = "Atendimento Infantil" },
                new Procedimento { Id = 11, Nome = "Raspagem" },
                new Procedimento { Id = 12, Nome = "Cirurgia Gengival" }
            );

            // Seed de EspecializacaoProcedimento (vínculos)
            modelBuilder.Entity<EspecializacaoProcedimento>().HasData(
                // Clínico Geral
                new EspecializacaoProcedimento { EspecializacaoId = 1, ProcedimentoId = 1 },
                new EspecializacaoProcedimento { EspecializacaoId = 1, ProcedimentoId = 2 },
                new EspecializacaoProcedimento { EspecializacaoId = 1, ProcedimentoId = 4 },
                new EspecializacaoProcedimento { EspecializacaoId = 1, ProcedimentoId = 5 },

                // Endodontia
                new EspecializacaoProcedimento { EspecializacaoId = 2, ProcedimentoId = 3 },

                // Ortodontia
                new EspecializacaoProcedimento { EspecializacaoId = 3, ProcedimentoId = 6 },
                new EspecializacaoProcedimento { EspecializacaoId = 3, ProcedimentoId = 7 },

                // Implantodontia
                new EspecializacaoProcedimento { EspecializacaoId = 4, ProcedimentoId = 8 },
                new EspecializacaoProcedimento { EspecializacaoId = 4, ProcedimentoId = 9 },

                // Odontopediatria
                new EspecializacaoProcedimento { EspecializacaoId = 5, ProcedimentoId = 10 },

                // Periodontia
                new EspecializacaoProcedimento { EspecializacaoId = 6, ProcedimentoId = 11 },
                new EspecializacaoProcedimento { EspecializacaoId = 6, ProcedimentoId = 12 }
            );

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            conexao = @"Server=localhost\SQLExpress;
                        Database=ClinicaJedi;
                        Integrated Security=True;
                        TrustServerCertificate=True";

            optionsBuilder.UseSqlServer(conexao);
            base.OnConfiguring(optionsBuilder);
        }
    }
}
