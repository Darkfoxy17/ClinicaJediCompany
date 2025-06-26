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

        //public ChamadosController(IWebHostEnvironment webHostEnvironment)
        //{
        //    servidorWeb = webHostEnvironment;
        //}

        public IActionResult Index()
        {
            bd = new BancoDados();
            var listaChamados = bd.Chamados
                .Include(c => c.Cliente)
                .Include(c => c.Tecnico)
                .ToList();

            return View(listaChamados);
        }

        [HttpPost]
        public IActionResult Index(string Busca)
        {
            bd = new BancoDados();
            var listaChamados = bd.Agendamentos
                .Include(c => c.Paciente)
                .Include(c => c.Dentista)
                .ToList();

            if (!string.IsNullOrEmpty(Busca))
            {
                listaChamados = listaChamados
                    .Where(u => u.DataSolicitacao.ToString("dd/MM/yyyy").Contains(Busca))
                    .ToList();
            }

            return View(listaChamados);
        }

        [HttpGet]
        public IActionResult Incluir()
        {
            bd = new BancoDados();
            var chamado = new Agendamento
            {
                DataSolicitacao = DateTime.Now // Preenche a data ao abrir o formulário
            };

            ViewBag.Tecnicos = new SelectList(bd.Tecnicos.ToList(), "Id", "Nome");
            ViewBag.Clientes = new SelectList(bd.Clientes.ToList(), "Id", "Nome");

            return View(chamado);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Incluir(Agendamento model)
        {
            bd = new BancoDados();

            ViewBag.Tecnicos = new SelectList(bd.Tecnicos.ToList(), "Id", "Nome", model.TecnicoId);
            ViewBag.Clientes = new SelectList(bd.Clientes.ToList(), "Id", "Nome", model.ClienteId);

            if (ModelState.IsValid)
            {
                model.DataSolicitacao = DateTime.Now; // Reforça a data no momento do post
                bd.Chamados.Add(model);
                bd.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(model);
        }


        [HttpGet]
        public IActionResult Alterar(int id)
        {
            bd = new BancoDados();
            var chamado = bd.Chamados
                .Include(c => c.Cliente)
                .Include(c => c.Tecnico)
                .FirstOrDefault(c => c.Id == id);

            if (chamado == null)
            {
                return NotFound();
            }

            ViewBag.Tecnicos = new SelectList(bd.Tecnicos.ToList(), "Id", "Nome", chamado.TecnicoId);
            ViewBag.Clientes = new SelectList(bd.Clientes.ToList(), "Id", "Nome", chamado.ClienteId);

            return View(chamado);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Alterar(Agendamento model)
        {
            if (ModelState.IsValid)
            {
                bd = new BancoDados();

                var chamadoExistente = bd.Chamados.FirstOrDefault(c => c.Id == model.Id);
                if (chamadoExistente == null)
                    return NotFound();

                // Atualize apenas os campos editáveis
                chamadoExistente.Problema = model.Problema;
                chamadoExistente.Ocorrencia = model.Ocorrencia;
                chamadoExistente.ValorTotal = model.ValorTotal;
                chamadoExistente.ClienteId = model.ClienteId;
                chamadoExistente.TecnicoId = model.TecnicoId;
                chamadoExistente.Concluido = model.Concluido;

                bd.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.Clientes = new SelectList(bd.Clientes.ToList(), "Id", "Nome", model.ClienteId);
            ViewBag.Tecnicos = new SelectList(bd.Tecnicos.ToList(), "Id", "Nome", model.TecnicoId);
            return View(model);
        }


        [HttpGet]
        public IActionResult Exibir(int id)
        {
            bd = new BancoDados();
            var chamado = bd.Chamados
                .Include(c => c.Cliente)
                .Include(c => c.Tecnico)
                .FirstOrDefault(c => c.Id == id);

            if (chamado == null)
                return NotFound();

            return View(chamado);
        }

        [HttpGet]
        public IActionResult Excluir(int id)
        {
            bd = new BancoDados();
            var chamado = bd.Chamados
                .Include(c => c.Cliente)
                .Include(c => c.Tecnico)
                .FirstOrDefault(c => c.Id == id);

            if (chamado == null)
                return NotFound();

            return View(chamado);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Excluir(Agendamento model)
        {
            bd = new BancoDados();
            var chamado = bd.Chamados.FirstOrDefault(c => c.Id == model.Id);

            if (chamado != null)
            {
                bd.Chamados.Remove(chamado);
                bd.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(model);
        }
    }
}
