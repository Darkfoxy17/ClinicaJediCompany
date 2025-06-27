using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebAppChamadosTI.Data;
using WebAppChamadosTI.Models;

namespace WebAppChamadosTI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrador, Tecnico, Cliente")]
    public class AgendamentosController : Controller
    {
        BancoDados bd;
        IWebHostEnvironment servidorWeb;

        public IActionResult Index()
        {
            bd = new BancoDados();
            var listaAgendamentos = bd.Agendamentos
                .Include(a => a.Paciente)
                .Include(a => a.Dentista)
                .ToList();

            return View(listaAgendamentos);
        }

        [HttpPost]
        public IActionResult Index(string busca)
        {
            bd = new BancoDados();
            var listaAgendamentos = bd.Agendamentos
                .Include(a => a.Paciente)
                .Include(a => a.Dentista)
                .ToList();

            if (!string.IsNullOrEmpty(busca))
            {
                listaAgendamentos = listaAgendamentos
                    .Where(a => a.Data.ToString("dd/MM/yyyy").Contains(busca))
                    .ToList();
            }

            return View(listaAgendamentos);
        }

        [HttpGet]
        public IActionResult Incluir()
        {
            bd = new BancoDados();
            var agendamento = new Agendamento
            {
                Data = DateTime.Now
            };

            ViewBag.Dentistas = new SelectList(bd.Dentistas.ToList(), "Id", "Nome");
            ViewBag.Pacientes = new SelectList(bd.Pacientes.ToList(), "Id", "Nome");

            return View(agendamento);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Incluir(Agendamento model)
        {
            bd = new BancoDados();

            ViewBag.Dentistas = new SelectList(bd.Dentistas.ToList(), "Id", "Nome", model.DentistaId);
            ViewBag.Pacientes = new SelectList(bd.Pacientes.ToList(), "Id", "Nome", model.PacienteId);

            if (ModelState.IsValid)
            {
                model.Data = DateTime.Now;
                bd.Agendamentos.Add(model);
                bd.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Alterar(int id)
        {
            bd = new BancoDados();
            var agendamento = bd.Agendamentos
                .Include(a => a.Paciente)
                .Include(a => a.Dentista)
                .FirstOrDefault(a => a.Id == id);

            if (agendamento == null)
            {
                return NotFound();
            }

            ViewBag.Dentistas = new SelectList(bd.Dentistas.ToList(), "Id", "Nome", agendamento.DentistaId);
            ViewBag.Pacientes = new SelectList(bd.Pacientes.ToList(), "Id", "Nome", agendamento.PacienteId);

            return View(agendamento);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Alterar(Agendamento model)
        {
            if (ModelState.IsValid)
            {
                bd = new BancoDados();
                var agendamentoExistente = bd.Agendamentos.FirstOrDefault(a => a.Id == model.Id);

                if (agendamentoExistente == null)
                    return NotFound();

                //agendamentoExistente.Motivo = model.Motivo;
                //agendamentoExistente.Observacao = model.Observacao;
                //agendamentoExistente.Valor = model.Valor;
                //agendamentoExistente.PacienteId = model.PacienteId;
                //agendamentoExistente.DentistaId = model.DentistaId;
                //agendamentoExistente.Concluido = model.Concluido;

                bd.SaveChanges();
                return RedirectToAction("Index");
            }

            bd = new BancoDados();
            ViewBag.Pacientes = new SelectList(bd.Pacientes.ToList(), "Id", "Nome", model.PacienteId);
            ViewBag.Dentistas = new SelectList(bd.Dentistas.ToList(), "Id", "Nome", model.DentistaId);
            return View(model);
        }

        [HttpGet]
        public IActionResult Exibir(int id)
        {
            bd = new BancoDados();
            var agendamento = bd.Agendamentos
                .Include(a => a.Paciente)
                .Include(a => a.Dentista)
                .FirstOrDefault(a => a.Id == id);

            if (agendamento == null)
                return NotFound();

            return View(agendamento);
        }

        [HttpGet]
        public IActionResult Excluir(int id)
        {
            bd = new BancoDados();
            var agendamento = bd.Agendamentos
                .Include(a => a.Paciente)
                .Include(a => a.Dentista)
                .FirstOrDefault(a => a.Id == id);

            if (agendamento == null)
                return NotFound();

            return View(agendamento);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Excluir(Agendamento model)
        {
            bd = new BancoDados();
            var agendamento = bd.Agendamentos.FirstOrDefault(a => a.Id == model.Id);

            if (agendamento != null)
            {
                bd.Agendamentos.Remove(agendamento);
                bd.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(model);
        }
    }
}
