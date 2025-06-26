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
            var listaClientes = bd.Clientes
                .Include(u => u.Usuario)
                .ToList();
            return View(listaClientes);
        }

        [HttpPost]
        public IActionResult Index(string busca)
        {
            bd = new BancoDados();
            var listaClientes = bd.Clientes
                .Include(u => u.Usuario)
                .ToList();
            if (!string.IsNullOrWhiteSpace(busca))
            {
                listaClientes = listaClientes
                    .Where(c =>
                        c.Nome.Contains(busca) ||
                        c.Profissao.Contains(busca) ||
                        c.Setor.Contains(busca) ||
                        c.Usuario.Email.Contains(busca))
                    .ToList();
            }
            return View(listaClientes);
        }

        [HttpGet]
        public IActionResult Incluir()
        {
            bd = new BancoDados();
            var listaUsuarios = bd.Usuarios.ToList();
            ViewBag.Usuarios = new SelectList(listaUsuarios, "Id", "Email");

            Paciente cliente = new Paciente();
            return View(cliente);
        }

        [HttpGet("Admin/Clientes/Incluir/{idusuario}")]
        public IActionResult Incluir(int idusuario)
        {
            bd = new BancoDados();
            var listaUsuarios = bd.Usuarios.Where(u => u.Id == idusuario).ToList();
            ViewBag.Usuarios = new SelectList(listaUsuarios, "Id", "Email", idusuario);

            Paciente cliente = new Paciente();
            return View(cliente);
        }

        [HttpPost("Admin/Clientes/Incluir/{idusuario}")]
        [ValidateAntiForgeryToken]
        public IActionResult Incluir(int idusuario, Paciente model)
        {
            bd = new BancoDados();
            if (ModelState.IsValid)
            {
                var cliente = bd.Clientes.FirstOrDefault(c => c.UsuarioId == model.UsuarioId);
                var tecnico = bd.Tecnicos.FirstOrDefault(t => t.UsuarioId == model.UsuarioId);

                if (cliente == null && tecnico == null)
                {
                    bd.Clientes.Add(model);
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
            var cliente = bd.Clientes
                .Include(c => c.Usuario)
                .FirstOrDefault(c => c.Id == id);

            if (cliente == null)
            {
                return NotFound();
            }

            var listaUsuarios = bd.Usuarios.ToList();
            ViewBag.Usuarios = new SelectList(listaUsuarios, "Id", "Email", cliente.UsuarioId);

            return View(cliente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Alterar(Paciente model, IFormFile? arquivo)
        {
            bd = new BancoDados();
            if (ModelState.IsValid)
            {
                var outroCliente = bd.Clientes
                    .FirstOrDefault(c => c.UsuarioId == model.UsuarioId && c.Id != model.Id);

                var tecnico = bd.Tecnicos
                    .FirstOrDefault(t => t.UsuarioId == model.UsuarioId);

                if (outroCliente == null && tecnico == null)
                {
                    var cliente = bd.Clientes
                        .Include(c => c.Usuario)
                        .FirstOrDefault(c => c.Id == model.Id);

                    if (cliente == null)
                        return NotFound();

                    cliente.Nome = model.Nome;
                    cliente.Profissao = model.Profissao;
                    cliente.Setor = model.Setor;
                    cliente.UsuarioId = model.UsuarioId;

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

                            if (!string.IsNullOrWhiteSpace(cliente.Usuario.Arquivo))
                                ExcluirArquivo(cliente.Usuario.Arquivo);

                            var nomeArquivo = SalvarArquivo(arquivo);
                            cliente.Usuario.Arquivo = nomeArquivo;
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
            var cliente = bd.Clientes
                .Include(c => c.Usuario)
                .FirstOrDefault(c => c.Id == id);
            return View(cliente);
        }

        [HttpGet]
        public IActionResult Excluir(int id)
        {
            bd = new BancoDados();
            var cliente = bd.Clientes
                .Include(c => c.Usuario)
                .FirstOrDefault(u => u.Id == id);
            if (cliente == null)
            {
                return NotFound();
            }
            return View(cliente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Excluir(Paciente model)
        {
            bd = new BancoDados();
            var cliente = bd.Clientes.FirstOrDefault(u => u.Id == model.Id);
            if (model.Id > 0)
            {
                bd.Clientes.Remove(cliente);
                bd.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }
    }
}
