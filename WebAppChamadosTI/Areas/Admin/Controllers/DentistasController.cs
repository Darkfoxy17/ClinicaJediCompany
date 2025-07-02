using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebAppChamadosTI.Data;
using WebAppChamadosTI.Models;

namespace WebAppChamadosTI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Atendente, Dentista")]
    public class DentistasController : Controller
    {
        BancoDados bd;

        public DentistasController() { }

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
        public IActionResult Alterar(Dentista model)
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
                        .FirstOrDefault(t => t.Id == model.Id);

                    if (dentista == null)
                        return NotFound();

                    dentista.Nome = model.Nome;
                    dentista.UsuarioId = model.UsuarioId;

                    bd.SaveChanges();
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError("UsuarioId", "Este e-mail já está vinculado a outro cadastro!");
            }

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
