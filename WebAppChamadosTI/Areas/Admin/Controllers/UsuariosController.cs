using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAppChamadosTI.Data;
using WebAppChamadosTI.Models;
using System.Drawing;

namespace WebAppChamadosTI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrador")]
    public class UsuariosController : Controller
    {
        BancoDados bd;
        IWebHostEnvironment servidorweb;

        public UsuariosController(IWebHostEnvironment webHostEnvironment)
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

        public IActionResult Index()
        {
            bd = new BancoDados();
            var listaUsuarios = bd.Usuarios.ToList();
            return View(listaUsuarios);
        }

        [HttpPost]
        public IActionResult Index(string busca)
        {
            bd = new BancoDados();
            var listaUsuarios = bd.Usuarios.ToList();

            if (!string.IsNullOrWhiteSpace(busca))
            {
                listaUsuarios = listaUsuarios
                    .Where(u => u.Email.Contains(busca))
                    .ToList();
            }

            return View(listaUsuarios);
        }

        [HttpGet]
        public IActionResult Incluir()
        {
            Usuario usuario = new Usuario();
            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Incluir(Usuario model, IFormFile? arquivo)
        {
            if (ModelState.IsValid)
            {
                bd = new BancoDados();
                var usuario = bd.Usuarios.FirstOrDefault(u => u.Email == model.Email);
                if (usuario != null)
                {
                    ModelState.AddModelError("Email", "Conta de e-mail já cadastrada!");
                    return View(usuario);
                }

                if (arquivo != null)
                {
                    var nomeArquivo = SalvarArquivo(arquivo);
                    model.Arquivo = nomeArquivo;
                }
                else
                {
                    model.Arquivo = "default-user.png";
                }

                bd.Usuarios.Add(model);
                bd.SaveChanges();

                if (model.Perfil == Perfil.Cliente)
                {
                    return RedirectToAction("Incluir", "Clientes", new { area = "Admin", idusuario = model.Id });
                }
                else
                {
                    return RedirectToAction("Incluir", "Tecnicos", new { area = "Admin", idusuario = model.Id });
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Alterar(int id)
        {
            bd = new BancoDados();
            var usuario = bd.Usuarios.FirstOrDefault(u => u.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Alterar(Usuario model, IFormFile arquivo)
        {
            if (ModelState.IsValid)
            {
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
                                return View(model);
                            }
                        }
                    }
                    catch
                    {
                        ModelState.AddModelError("Arquivo", "Arquivo de imagem inválido.");
                        return View(model);
                    }

                    // Exclui imagem antiga, se houver
                    ExcluirArquivo(model.Arquivo);

                    // Salva nova imagem
                    var nomeArquivo = SalvarArquivo(arquivo);
                    model.Arquivo = nomeArquivo;
                }

                bd = new BancoDados();
                bd.Usuarios.Update(model);
                bd.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(model);
        }


        [HttpGet]
        public IActionResult Exibir(int id)
        {
            bd = new BancoDados();
            var usuario = bd.Usuarios.FirstOrDefault(u => u.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        [HttpGet]
        public IActionResult Excluir(int id)
        {
            bd = new BancoDados();
            var usuario = bd.Usuarios.FirstOrDefault(u => u.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Excluir(Usuario model)
        {
            bd = new BancoDados();
            var usuario = bd.Usuarios.FirstOrDefault(u => u.Id == model.Id);
            if (model.Id > 0)
            {
                if (!string.IsNullOrWhiteSpace(model.Arquivo))
                {
                    ExcluirArquivo(model.Arquivo);
                }

                bd.Usuarios.Remove(usuario);
                bd.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(model);
        }
    }
}
