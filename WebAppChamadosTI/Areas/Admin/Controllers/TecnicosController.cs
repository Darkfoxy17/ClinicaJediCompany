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
    public class TecnicosController : Controller
    {
        BancoDados bd;
        IWebHostEnvironment servidorweb;

        public TecnicosController(IWebHostEnvironment webHostEnvironment)
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
            var listaTecnicos = bd.Tecnicos
                .Include(u => u.Usuario)
                .ToList();
            return View(listaTecnicos);
        }

        [HttpPost]
        public IActionResult Index(string busca)
        {
            bd = new BancoDados();
            var listaTecnicos = bd.Tecnicos
                .Include(u => u.Usuario)
                .ToList();

            if (!string.IsNullOrWhiteSpace(busca))
            {
                listaTecnicos = listaTecnicos
                    .Where(t =>
                        t.Nome.Contains(busca) ||
                        t.Especialidade.Contains(busca) ||
                        t.Usuario.Email.Contains(busca))
                    .ToList();
            }

            return View(listaTecnicos);
        }

        [HttpGet]
        public IActionResult Incluir()
        {
            bd = new BancoDados();
            var listaUsuarios = bd.Usuarios.ToList();
            ViewBag.Usuarios = new SelectList(listaUsuarios, "Id", "Email");

            return View(new Tecnico());
        }

        [HttpGet("Admin/Tecnicos/Incluir/{idusuario}")]
        public IActionResult Incluir(int idusuario)
        {
            bd = new BancoDados();
            var listaUsuarios = bd.Usuarios.Where(u => u.Id == idusuario).ToList();
            ViewBag.Usuarios = new SelectList(listaUsuarios, "Id", "Email", idusuario);

            return View(new Tecnico());
        }

        [HttpPost("Admin/Tecnicos/Incluir/{idusuario}")]
        [ValidateAntiForgeryToken]
        public IActionResult Incluir(int idusuario, Tecnico model)
        {
            bd = new BancoDados();
            if (ModelState.IsValid)
            {
                var tecnico = bd.Tecnicos.FirstOrDefault(t => t.UsuarioId == model.UsuarioId);
                var cliente = bd.Clientes.FirstOrDefault(c => c.UsuarioId == model.UsuarioId);

                if (tecnico == null && cliente == null)
                {
                    bd.Tecnicos.Add(model);
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
            var tecnico = bd.Tecnicos
                .Include(t => t.Usuario)
                .FirstOrDefault(t => t.Id == id);

            if (tecnico == null)
                return NotFound();

            var listaUsuarios = bd.Usuarios.ToList();
            ViewBag.Usuarios = new SelectList(listaUsuarios, "Id", "Email", tecnico.UsuarioId);

            return View(tecnico);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Alterar(Tecnico model, IFormFile? arquivo)
        {
            bd = new BancoDados();
            if (ModelState.IsValid)
            {
                var outroTecnico = bd.Tecnicos
                    .FirstOrDefault(t => t.UsuarioId == model.UsuarioId && t.Id != model.Id);
                var cliente = bd.Clientes
                    .FirstOrDefault(c => c.UsuarioId == model.UsuarioId);

                if (outroTecnico == null && cliente == null)
                {
                    var tecnico = bd.Tecnicos
                        .Include(t => t.Usuario)
                        .FirstOrDefault(t => t.Id == model.Id);

                    if (tecnico == null)
                        return NotFound();

                    tecnico.Nome = model.Nome;
                    tecnico.Especialidade = model.Especialidade;
                    tecnico.UsuarioId = model.UsuarioId;

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

                            if (!string.IsNullOrWhiteSpace(tecnico.Usuario.Arquivo))
                                ExcluirArquivo(tecnico.Usuario.Arquivo);

                            var nomeArquivo = SalvarArquivo(arquivo);
                            tecnico.Usuario.Arquivo = nomeArquivo;
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
            var tecnico = bd.Tecnicos
                .Include(t => t.Usuario)
                .FirstOrDefault(t => t.Id == id);

            return View(tecnico);
        }

        [HttpGet]
        public IActionResult Excluir(int id)
        {
            bd = new BancoDados();
            var tecnico = bd.Tecnicos
                .Include(t => t.Usuario)
                .FirstOrDefault(t => t.Id == id);

            if (tecnico == null)
                return NotFound();

            return View(tecnico);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Excluir(Tecnico model)
        {
            bd = new BancoDados();
            var tecnico = bd.Tecnicos.FirstOrDefault(t => t.Id == model.Id);

            if (tecnico != null)
            {
                bd.Tecnicos.Remove(tecnico);
                bd.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(model);
        }
    }
}
