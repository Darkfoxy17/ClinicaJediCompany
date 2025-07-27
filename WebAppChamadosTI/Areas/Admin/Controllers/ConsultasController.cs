using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebAppChamadosTI.Data;
using WebAppChamadosTI.Models;

namespace WebAppChamadosTI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Dentista, Paciente")]
    public class ConsultasController : Controller
    {
        private BancoDados bd = new BancoDados();

        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("AcessoNegado", "Home");

            var email = User.Identity.Name;

            var consultas = bd.Agendamentos
                .Include(a => a.Paciente).ThenInclude(p => p.Usuario)
                .Include(a => a.Dentista).ThenInclude(d => d.Usuario)
                .Include(a => a.Procedimento)
                .Include(a => a.StatusAgendamento)
                .AsQueryable();

            if (User.IsInRole("Paciente"))
            {
                var paciente = bd.Pacientes.Include(p => p.Usuario)
                    .FirstOrDefault(p => p.Usuario.Email == email);

                if (paciente == null)
                    return RedirectToAction("AcessoNegado", "Home");

                consultas = consultas.Where(a => a.PacienteId == paciente.Id);
            }
            else if (User.IsInRole("Dentista"))
            {
                var dentista = bd.Dentistas.Include(d => d.Usuario)
                    .FirstOrDefault(d => d.Usuario.Email == email);

                if (dentista == null)
                    return RedirectToAction("AcessoNegado", "Home");

                consultas = consultas.Where(a => a.DentistaId == dentista.Id);
            }

            return View(consultas.ToList());
        }


        public IActionResult Detalhes(int id)
        {
            var consulta = bd.Agendamentos
                .Include(a => a.Paciente).ThenInclude(p => p.Usuario)
                .Include(a => a.Dentista).ThenInclude(d => d.Usuario)
                .Include(a => a.Procedimento)
                .Include(a => a.StatusAgendamento)
                .FirstOrDefault(a => a.Id == id);

            if (consulta == null)
                return NotFound();

            // Verifica se o usuário tem permissão para ver essa consulta
            var email = User.Identity.Name;
            var isDentista = User.IsInRole("Dentista");
            var isPaciente = User.IsInRole("Paciente");

            if (isPaciente && consulta.Paciente.Usuario.Email != email)
                return RedirectToAction("AcessoNegado", "Home");

            if (isDentista && consulta.Dentista.Usuario.Email != email)
                return RedirectToAction("AcessoNegado", "Home");

            ViewBag.MostrarObservacao = isDentista;

            return View(consulta);
        }
    }
}
