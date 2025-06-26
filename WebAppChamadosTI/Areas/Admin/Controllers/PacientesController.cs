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
    public class PacientesController : Controller
    {
        BancoDados bd;
        IWebHostEnvironment servidorweb;

        public PacientesController(IWebHostEnvironment webHostEnvironment)
        {
            servidorweb = webHostEnvironment;
        }

        public string SalvarArquivo(IFormFile arquivo)
        {
            if (arquivo == null)
            {
                return string.Empty;
            }

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
            {
                return false;
            }

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
            var listaPacientes = bd.Pacientes
                .Include(u => u.Usuario)
                .ToList();
            return View(listaPacientes);
        }

        [HttpPost]
        public IActionResult Index(string busca)
        {
            bd = new BancoDados();
            var listaPacientes = bd.Pacientes
                .Include(u => u.Usuario)
                .ToList();
            if (!string.IsNullOrWhiteSpace(busca))
            {
                listaPacientes = listaPacientes
                    .Where(c =>
                        c.Nome.Contains(busca) ||
                        c.Profissao.Contains(busca) ||
                        c.Setor.Contains(busca) ||
                        c.Usuario.Email.Contains(busca))
                    .ToList();
            }
            return View(listaPacientes);
        }

        [HttpGet]
        public IActionResult Incluir()
        {
            bd = new BancoDados();
            var listaUsuarios = bd.Usuarios.ToList();
            ViewBag.Usuarios = new SelectList(listaUsuarios, "Id", "Email");

            Paciente paciente = new Paciente();
            return View(paciente);
        }

        [HttpGet("Admin/Pacientes/Incluir/{idusuario}")]
        public IActionResult Incluir(int idusuario)
        {
            bd = new BancoDados();
            var listaUsuarios = bd.Usuarios.Where(u => u.Id == idusuario).ToList();
            ViewBag.Usuarios = new SelectList(listaUsuarios, "Id", "Email", idusuario);

            Paciente paciente = new Paciente();
            return View(paciente);
        }

        [HttpPost("Admin/Pacientes/Incluir/{idusuario}")]
        [ValidateAntiForgeryToken]
        public IActionResult Incluir(int idusuario, Paciente model)
        {
            bd = new BancoDados();
            if (ModelState.IsValid)
            {
                var paciente = bd.Pacientes.FirstOrDefault(c => c.UsuarioId == model.UsuarioId);
                var dentista = bd.Dentistas.FirstOrDefault(t => t.UsuarioId == model.UsuarioId);

                if (paciente == null && dentista == null)
                {
                    bd.Pacientes.Add(model);
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
            var paciente = bd.Pacientes
                .Include(c => c.Usuario)
                .FirstOrDefault(c => c.Id == id);

            if (paciente == null)
            {
                return NotFound();
            }

            var listaUsuarios = bd.Usuarios.ToList();
            ViewBag.Usuarios = new SelectList(listaUsuarios, "Id", "Email", paciente.UsuarioId);

            return View(paciente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Alterar(Paciente model, IFormFile? arquivo)
        {
            bd = new BancoDados();
            if (ModelState.IsValid)
            {
                var outroPaciente = bd.Pacientes
                    .FirstOrDefault(c => c.UsuarioId == model.UsuarioId && c.Id != model.Id);

                var dentista = bd.Dentistas
                    .FirstOrDefault(t => t.UsuarioId == model.UsuarioId);

                if (outroPaciente == null && dentista == null)
                {
                    var paciente = bd.Pacientes
                        .Include(c => c.Usuario)
                        .FirstOrDefault(c => c.Id == model.Id);

                    if (paciente == null)
                        return NotFound();

                    paciente.Nome = model.Nome;
                    paciente.Profissao = model.Profissao;
                    paciente.Setor = model.Setor;
                    paciente.UsuarioId = model.UsuarioId;

                    if (arquivo != null)
                    {
                        try
                        {
                            using (var stream = arquivo.OpenReadStream())
                            using (var image = System.Drawing.Image.FromStream(stream))
                            {
                                if (image.Width > 150 || image.Height > 150)
                                {
                                    ModelState.AddModelError("Arquivo", "A imagem não pode ter mais que 150x150 pixels.");
                                    goto RetornaErro;
                                }
                            }

                            if (!string.IsNullOrWhiteSpace(paciente.Usuario.Arquivo))
                                ExcluirArquivo(paciente.Usuario.Arquivo);

                            var nomeArquivo = SalvarArquivo(arquivo);
                            paciente.Usuario.Arquivo = nomeArquivo;
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
            var paciente = bd.Pacientes
                .Include(c => c.Usuario)
                .FirstOrDefault(c => c.Id == id);
            return View(paciente);
        }

        [HttpGet]
        public IActionResult Excluir(int id)
        {
            bd = new BancoDados();
            var paciente = bd.Pacientes
                .Include(c => c.Usuario)
                .FirstOrDefault(u => u.Id == id);
            if (paciente == null)
            {
                return NotFound();
            }
            return View(paciente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Excluir(Paciente model)
        {
            bd = new BancoDados();
            var paciente = bd.Pacientes.FirstOrDefault(u => u.Id == model.Id);
            if (model.Id > 0)
            {
                bd.Pacientes.Remove(paciente);
                bd.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }
    }
}
