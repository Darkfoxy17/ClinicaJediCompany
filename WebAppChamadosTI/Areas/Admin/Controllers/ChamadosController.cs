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
    public class ChamadosController : Controller
    {
        BancoDados bd;
        IWebHostEnvironment servidorWeb;

        public ChamadosController(IWebHostEnvironment webHostEnvironment)
        {
            servidorWeb = webHostEnvironment;
        }

        private string SalvarArquivo(IFormFile arquivo)
        {
            if (arquivo == null)
                return string.Empty;

            string nomeArquivo = $"{Guid.NewGuid()}{Path.GetExtension(arquivo.FileName)}";
            string pastaArquivo = Path.Combine(servidorWeb.WebRootPath, "uploads");
            string caminhoArquivo = Path.Combine(pastaArquivo, nomeArquivo);

            using var dadosArquivo = new FileStream(caminhoArquivo, FileMode.Create);
            arquivo.CopyTo(dadosArquivo);

            return nomeArquivo;
        }

        private bool ExcluirArquivo(string nomeArquivo)
        {
            if (string.IsNullOrWhiteSpace(nomeArquivo))
                return false;

            string pastaArquivo = Path.Combine(servidorWeb.WebRootPath, "uploads");
            string caminhoArquivo = Path.Combine(pastaArquivo, nomeArquivo);
            System.IO.File.Delete(caminhoArquivo);

            return true;
        }

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
            var listaChamados = bd.Chamados
                .Include(c => c.Cliente)
                .Include(c => c.Tecnico)
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
            var chamado = new Chamado
            {
                DataSolicitacao = DateTime.Now // Preenche a data ao abrir o formulário
            };

            ViewBag.Tecnicos = new SelectList(bd.Tecnicos.ToList(), "Id", "Nome");
            ViewBag.Clientes = new SelectList(bd.Clientes.ToList(), "Id", "Nome");

            return View(chamado);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Incluir(Chamado model)
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
        public IActionResult Alterar(Chamado model)
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
        public IActionResult Excluir(Chamado model)
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
