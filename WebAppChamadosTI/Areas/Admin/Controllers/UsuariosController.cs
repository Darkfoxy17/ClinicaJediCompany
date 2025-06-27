using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAppChamadosTI.Data;
using WebAppChamadosTI.Models;

namespace WebAppChamadosTI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Atendente")]
    public class UsuariosController : Controller
    {
        BancoDados bd;

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
        public IActionResult Incluir(Usuario model)
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

                bd.Usuarios.Add(model);
                bd.SaveChanges();

                if (model.Perfil == Perfil.Paciente)
                {
                    return RedirectToAction("Incluir", "Pacientes", new { area = "Admin", idusuario = model.Id });
                }
                else
                {
                    return RedirectToAction("Incluir", "Dentistas", new { area = "Admin", idusuario = model.Id });
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
        public IActionResult Alterar(Usuario model)
        {
            if (ModelState.IsValid)
            {
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
            if (usuario != null)
            {
                bd.Usuarios.Remove(usuario);
                bd.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(model);
        }
    }
}
