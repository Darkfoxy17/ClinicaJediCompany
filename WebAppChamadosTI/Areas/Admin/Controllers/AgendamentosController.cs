using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebAppChamadosTI.Data;
using WebAppChamadosTI.Models;

namespace WebAppChamadosTI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Atendente")]
    public class AgendamentosController : Controller
    {
        private BancoDados bd = new BancoDados();

        public IActionResult Index(string busca)
        {
            var query = bd.Agendamentos
                .Include(a => a.Paciente)
                .Include(a => a.Dentista)
                .Include(a => a.Procedimento)
                .Include(a => a.StatusAgendamento)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(busca))
            {
                busca = busca.ToLower();

                // Tenta interpretar a busca como data
                if (DateTime.TryParse(busca, out DateTime dataBusca))
                {
                    query = query.Where(a => a.Data.Date == dataBusca.Date);
                }
                else
                {
                    query = query.Where(a =>
                        a.Paciente.Nome.ToLower().Contains(busca) ||
                        a.Dentista.Nome.ToLower().Contains(busca) ||
                        a.Procedimento.Nome.ToLower().Contains(busca) ||
                        a.StatusAgendamento.Nome.ToLower().Contains(busca)
                    );
                }
            }

            var lista = query.ToList();
            return View(lista);
        }



        public IActionResult Incluir()
        {
            ViewBag.Procedimentos = new SelectList(bd.Procedimentos.ToList(), "Id", "Nome");
            ViewBag.Dentistas = new SelectList(Enumerable.Empty<SelectListItem>());
            ViewBag.Pacientes = new SelectList(bd.Pacientes.Include(p => p.Usuario).ToList(), "Id", "Nome");
            return View();
        }



        [HttpPost]
        public IActionResult Incluir(Agendamento agendamento)
        {
            if (ModelState.IsValid)
            {
                agendamento.StatusAgendamentoId = 1; // Define como "Pendente" automaticamente
                bd.Agendamentos.Add(agendamento);
                bd.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Pacientes = new SelectList(bd.Pacientes.Include(p => p.Usuario).ToList(), "Id", "Nome", agendamento.PacienteId);
            ViewBag.Dentistas = new SelectList(bd.Dentistas.Include(d => d.Usuario).ToList(), "Id", "Nome", agendamento.DentistaId);
            ViewBag.Pacientes = new SelectList(bd.Pacientes.ToList(), "Id", "Nome");

            return View(agendamento);
        }

        public IActionResult Alterar(int id)
        {
            var agendamento = bd.Agendamentos.Find(id);
            if (agendamento == null)
                return NotFound();

            ViewBag.Pacientes = new SelectList(bd.Pacientes.Include(p => p.Usuario).ToList(), "Id", "Nome", agendamento.PacienteId);
            ViewBag.Dentistas = new SelectList(bd.Dentistas.Include(d => d.Usuario).ToList(), "Id", "Nome", agendamento.DentistaId);
            ViewBag.Procedimentos = new SelectList(bd.Procedimentos, "Id", "Nome", agendamento.ProcedimentoId);
            ViewBag.Statuses = new SelectList(bd.StatusAgendamentos, "Id", "Nome", agendamento.StatusAgendamentoId);

            return View(agendamento);
        }

        [HttpPost]
        public IActionResult Alterar(Agendamento agendamento)
        {
            if (ModelState.IsValid)
            {
                var agendamentoExistente = bd.Agendamentos.Find(agendamento.Id);

                if (agendamentoExistente == null)
                    return NotFound();

                // Atualiza apenas os campos necessários
                agendamentoExistente.StatusAgendamentoId = agendamento.StatusAgendamentoId;
                agendamentoExistente.ProcedimentoId = agendamento.ProcedimentoId;
                agendamentoExistente.DentistaId = agendamento.DentistaId;
                agendamentoExistente.PacienteId = agendamento.PacienteId;
                agendamentoExistente.Data = agendamento.Data;

                bd.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.Pacientes = new SelectList(bd.Pacientes.Include(p => p.Usuario).ToList(), "Id", "Nome", agendamento.PacienteId);
            ViewBag.Dentistas = new SelectList(bd.Dentistas.Include(d => d.Usuario).ToList(), "Id", "Nome", agendamento.DentistaId);
            ViewBag.Procedimentos = new SelectList(bd.Procedimentos, "Id", "Nome", agendamento.ProcedimentoId);
            ViewBag.Statuses = new SelectList(bd.StatusAgendamentos, "Id", "Nome", agendamento.StatusAgendamentoId);

            return View(agendamento);
        }


        public IActionResult Exibir(int id)
        {
            var agendamento = bd.Agendamentos
                .Include(a => a.Paciente)
                .Include(a => a.Dentista)
                .Include(a => a.Procedimento)
                .Include(a => a.StatusAgendamento)
                .FirstOrDefault(a => a.Id == id);

            if (agendamento == null)
                return NotFound();

            return View(agendamento);
        }


        public IActionResult Excluir(int id)
        {
            var agendamento = bd.Agendamentos
                .Include(a => a.Paciente)
                .Include(a => a.Dentista)
                .FirstOrDefault(a => a.Id == id);

            if (agendamento == null)
                return NotFound();

            return View(agendamento);
        }

        [HttpPost, ActionName("Excluir")]
        public IActionResult ConfirmarExclusao(int id)
        {
            var agendamento = bd.Agendamentos.Find(id);

            if (agendamento == null)
                return NotFound();

            bd.Agendamentos.Remove(agendamento);
            bd.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public JsonResult ObterDentistasPorProcedimento(int procedimentoId)
        {
            var dentistas = bd.DentistaProcedimentos
                .Where(dp => dp.ProcedimentoId == procedimentoId)
                .Select(dp => new
                {
                    dp.Dentista.Id,
                    dp.Dentista.Nome
                })
                .Distinct()
                .ToList();

            return Json(dentistas);
        }

    }
}
