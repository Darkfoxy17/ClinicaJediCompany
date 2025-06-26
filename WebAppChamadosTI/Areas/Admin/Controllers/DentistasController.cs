using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebAppChamadosTI.Data;
using WebAppChamadosTI.Models;
using System.Drawing;

namespace WebAppChamadosTI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrador, Tecnico")]
    public class DentistasController : Controller
    {
        BancoDados bd;
        IWebHostEnvironment servidorweb;

        public DentistasController(IWebHostEnvironment webHostEnvironment)
        {
            servidorweb = webHostEnvironment;
        }

        public string SalvarArquivo(IFormFile arquivo)
        {
            if (arquivo == null)
                return string.Empty;

            string extensao = Path.GetExtension(arquivo.FileName).TrimStart('.');
            string nomeArquivo = $"{Guid.NewGuid()}.{extensao}";
            string pastaArquivo = Path.Combine(servidorweb.WebRootPath, "uploads");
            string caminhoArquivo = Path.Combine(pastaArquivo, nomeArquivo);

            using (var dadosArquivo = new FileStream(caminhoArquivo, FileMode.Create))
            {
                arquivo.CopyTo(dadosArquivo);
            }

            return nomeArquivo;
        }

        private bool ExcluirArquivo(string nomeArquivo)
        {
            if (string.IsNullOrEmpty(nomeArquivo) || nomeArquivo == "default-user.png")
                return false;

            string pastaArquivo = Path.Combine(servidorweb.WebRootPath, "uploads");
            string caminhoArquivo = Path.Combine(pastaArquivo, nomeArquivo);

            if (System.IO.File.Exists(caminhoArquivo))
            {
                System.IO.File.Delete(caminhoArquivo);
                return true;
            }

            return false;
        }

        [HttpGet]
        public IActionResult Index()
        {
            bd = new BancoDados();
            var listaDentistas = bd.Dentistas
                .Include(u => u.Usuario)
                .ToList();
            return View(listaDentistas);
        }

        [HttpPost]
        public IActionResult Index(string busca)
        {
            bd = new BancoDados();
            var listaDentistas = bd.Dentistas
                .Include(u => u.Usuario)
                .ToList();

            if (!string.IsNullOrWhiteSpace(busca))
            {
                listaDentistas = listaDentistas
                    .Where(t =>
                        t.Nome.Contains(busca) ||
                        t.Especialidade.Contains(busca) ||
                        t.Usuario.Email.Contains(busca))
                    .ToList();
            }

            return View(listaDentistas);
        }

        [HttpGet]
        public IActionResult Incluir()
        {
            bd = new BancoDados();
            var listaUsuarios = bd.Usuarios.ToList();
            ViewBag.Usuarios = new SelectList(listaUsuarios, "Id", "Email");

            return View(new Dentista());
        }

        [HttpGet("Admin/Dentistas/Incluir/{idusuario}")]
        public IActionResult Incluir(int idusuario)
        {
            bd = new BancoDados();
            var listaUsuarios = bd.Usuarios.Where(u => u.Id == idusuario).ToList();
            ViewBag.Usuarios = new SelectList(listaUsuarios, "Id", "Email", idusuario);

            return View(new Dentista());
        }

        [HttpPost("Admin/Dentistas/Incluir/{idusuario}")]
        [ValidateAntiForgeryToken]
        public IActionResult Incluir(int idusuario, Dentista model)
        {
            bd = new BancoDados();
            if (ModelState.IsValid)
            {
                var dentista = bd.Dentistas.FirstOrDefault(t => t.UsuarioId == model.UsuarioId);
                var paciente = bd.Pacientes.FirstOrDefault(c => c.UsuarioId == model.UsuarioId);

                if (dentista == null && paciente == null)
                {
                    bd.Dentistas.Add(model);
                    bd.SaveChanges();
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError("Nome", "Já possui cadastro vinculado!");
            }

            var listaUsuarios = bd.Usuarios.ToList();
            ViewBag.Usuarios = new SelectList(listaUsuarios, "Id", "Email", model.UsuarioId);
            return View(model);
        }

        [HttpGet]
        public IActionResult Alterar(int id)
        {
            bd = new BancoDados();
            var dentista = bd.Dentistas
                .Include(t => t.Usuario)
                .FirstOrDefault(t => t.Id == id);

            if (dentista == null)
                return NotFound();

            var listaUsuarios = bd.Usuarios.ToList();
            ViewBag.Usuarios = new SelectList(listaUsuarios, "Id", "Email", dentista.UsuarioId);

            return View(dentista);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Alterar(Dentista model, IFormFile? arquivo)
        {
            bd = new BancoDados();
            if (ModelState.IsValid)
            {
                var outroDentista = bd.Dentistas
                    .FirstOrDefault(t => t.UsuarioId == model.UsuarioId && t.Id != model.Id);
                var paciente = bd.Pacientes
                    .FirstOrDefault(c => c.UsuarioId == model.UsuarioId);

                if (outroDentista == null && paciente == null)
                {
                    var dentista = bd.Dentistas
                        .Include(t => t.Usuario)
                        .FirstOrDefault(t => t.Id == model.Id);

                    if (dentista == null)
                        return NotFound();

                    dentista.Nome = model.Nome;
                    dentista.Especialidade = model.Especialidade;
                    dentista.UsuarioId = model.UsuarioId;

                    if (arquivo != null)
                    {
                        try
                        {
                            using (var stream = arquivo.OpenReadStream())
                            using (var image = Image.FromStream(stream))
                            {
                                if (image.Width > 150 || image.Height > 150)
                                {
                                    ModelState.AddModelError("Arquivo", "A imagem não pode ter mais que 150x150 pixels.");
                                    goto RetornaErro;
                                }
                            }

                            if (!string.IsNullOrWhiteSpace(dentista.Usuario.Arquivo))
                                ExcluirArquivo(dentista.Usuario.Arquivo);

                            var nomeArquivo = SalvarArquivo(arquivo);
                            dentista.Usuario.Arquivo = nomeArquivo;
                        }
                        catch
                        {
                            ModelState.AddModelError("Arquivo", "Arquivo de imagem inválido.");
                            goto RetornaErro;
                        }
                    }

                    bd.SaveChanges();
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError("UsuarioId", "Este e-mail já está vinculado a outro cadastro!");
            }

        RetornaErro:
            var listaUsuarios = bd.Usuarios.ToList();
            ViewBag.Usuarios = new SelectList(listaUsuarios, "Id", "Email", model.UsuarioId);
            return View(model);
        }

        [HttpGet]
        public IActionResult Exibir(int id)
        {
            bd = new BancoDados();
            var dentista = bd.Dentistas
                .Include(t => t.Usuario)
                .FirstOrDefault(t => t.Id == id);

            return View(dentista);
        }

        [HttpGet]
        public IActionResult Excluir(int id)
        {
            bd = new BancoDados();
            var dentista = bd.Dentistas
                .Include(t => t.Usuario)
                .FirstOrDefault(t => t.Id == id);

            if (dentista == null)
                return NotFound();

            return View(dentista);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Excluir(Dentista model)
        {
            bd = new BancoDados();
            var dentista = bd.Dentistas.FirstOrDefault(t => t.Id == model.Id);

            if (dentista != null)
            {
                bd.Dentistas.Remove(dentista);
                bd.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(model);
        }
    }
}
