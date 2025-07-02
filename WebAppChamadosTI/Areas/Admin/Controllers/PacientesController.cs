using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebAppChamadosTI.Data;
using WebAppChamadosTI.Models;

namespace WebAppChamadosTI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Atendente")]
    public class PacientesController : Controller
    {
        BancoDados bd;

        [HttpGet]
        public IActionResult Index()
        {
            bd = new BancoDados();
            var listaPacientes = bd.Pacientes.Include(c => c.Usuario).ToList();
            return View(listaPacientes);
        }

        [HttpPost]
        public IActionResult Index(string busca)
        {
            bd = new BancoDados();
            var lista = bd.Pacientes.Include(c => c.Usuario).ToList();

            if (!string.IsNullOrWhiteSpace(busca))
            {
                lista = lista
                    .Where(c => c.Nome.Contains(busca) || c.Usuario.Email.Contains(busca))
                    .ToList();
            }

            return View(lista);
        }

        //[HttpGet]
        //public IActionResult Incluir()
        //{
        //    bd = new BancoDados();
        //    ViewBag.Usuarios = new SelectList(bd.Usuarios.ToList(), "Id", "Email");
        //    return View(new Paciente());
        //}

        //[HttpGet("Admin/Pacientes/Incluir/{idusuario}")]
        //public IActionResult Incluir(int idusuario)
        //{
        //    bd = new BancoDados();
        //    var usuarios = bd.Usuarios.Where(u => u.Id == idusuario).ToList();
        //    ViewBag.Usuarios = new SelectList(usuarios, "Id", "Email", idusuario);
        //    return View(new Paciente());
        //}

        //[HttpPost("Admin/Pacientes/Incluir/{idusuario}")]
        //[ValidateAntiForgeryToken]
        //public IActionResult Incluir(int idusuario, Paciente model)
        //{
        //    bd = new BancoDados();
        //    if (ModelState.IsValid)
        //    {
        //        var jaCadastrado = bd.Pacientes.Any(c => c.UsuarioId == model.UsuarioId)
        //                        || bd.Dentistas.Any(t => t.UsuarioId == model.UsuarioId);

        //        if (!jaCadastrado)
        //        {
        //            bd.Pacientes.Add(model);
        //            bd.SaveChanges();
        //            return RedirectToAction("Index");
        //        }

        //        ModelState.AddModelError("Nome", "Já possui cadastro vinculado!");
        //    }

        //    ViewBag.Usuarios = new SelectList(bd.Usuarios.ToList(), "Id", "Email", model.UsuarioId);
        //    return View(model);
        //}

        [HttpGet]
        public IActionResult Alterar(int id)
        {
            bd = new BancoDados();
            var paciente = bd.Pacientes.Include(c => c.Usuario).FirstOrDefault(c => c.Id == id);
            if (paciente == null) return NotFound();

            ViewBag.Usuarios = new SelectList(bd.Usuarios.ToList(), "Id", "Email", paciente.UsuarioId);
            return View(paciente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Alterar(Paciente model)
        {
            bd = new BancoDados();
            if (ModelState.IsValid)
            {
                var conflito = bd.Pacientes.Any(c => c.UsuarioId == model.UsuarioId && c.Id != model.Id)
                            || bd.Dentistas.Any(t => t.UsuarioId == model.UsuarioId);

                if (!conflito)
                {
                    var paciente = bd.Pacientes.FirstOrDefault(c => c.Id == model.Id);
                    if (paciente == null) return NotFound();

                    paciente.Nome = model.Nome;
                    paciente.UsuarioId = model.UsuarioId;

                    bd.SaveChanges();
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError("UsuarioId", "Este e-mail já está vinculado a outro cadastro!");
            }

            ViewBag.Usuarios = new SelectList(bd.Usuarios.ToList(), "Id", "Email", model.UsuarioId);
            return View(model);
        }

        [HttpGet]
        public IActionResult Exibir(int id)
        {
            bd = new BancoDados();
            var paciente = bd.Pacientes.Include(c => c.Usuario).FirstOrDefault(c => c.Id == id);
            return View(paciente);
        }

        [HttpGet]
        public IActionResult Excluir(int id)
        {
            bd = new BancoDados();
            var paciente = bd.Pacientes.Include(c => c.Usuario).FirstOrDefault(c => c.Id == id);
            if (paciente == null) return NotFound();

            return View(paciente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Excluir(Paciente model)
        {
            bd = new BancoDados();
            var paciente = bd.Pacientes.FirstOrDefault(c => c.Id == model.Id);
            if (paciente != null)
            {
                bd.Pacientes.Remove(paciente);
                bd.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(model);
        }
    }
}
