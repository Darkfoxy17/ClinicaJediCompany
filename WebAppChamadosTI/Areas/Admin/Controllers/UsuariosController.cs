//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using WebAppChamadosTI.Data;
//using WebAppChamadosTI.Models;
//using System.Linq;

//namespace WebAppChamadosTI.Areas.Admin.Controllers
//{
//    [Area("Admin")]
//    [Authorize(Roles = "Atendente")]
//    public class UsuariosController : Controller
//    {
//        BancoDados bd;

//        [HttpGet]
//        public IActionResult Index()
//        {
//            bd = new BancoDados();
//            var listaUsuarios = bd.Usuarios.ToList();
//            return View(listaUsuarios);
//        }

//        [HttpPost]
//        public IActionResult Index(string busca)
//        {
//            bd = new BancoDados();
//            var listaUsuarios = bd.Usuarios.ToList();

//            if (!string.IsNullOrWhiteSpace(busca))
//            {
//                listaUsuarios = listaUsuarios
//                    .Where(u => u.Email.Contains(busca))
//                    .ToList();
//            }

//            return View(listaUsuarios);
//        }

//        [HttpGet]
//        public IActionResult Incluir()
//        {
//            // Cria um usuário já com Perfil Dentista
//            return View(new Usuario { Perfil = Perfil.Dentista });
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public IActionResult Incluir(Usuario model)
//        {
//            if (ModelState.IsValid)
//            {
//                bd = new BancoDados();

//                var usuarioExistente = bd.Usuarios.FirstOrDefault(u => u.Email == model.Email);
//                if (usuarioExistente != null)
//                {
//                    ModelState.AddModelError("Email", "Conta de e-mail já cadastrada!");
//                    return View(model);
//                }

//                // Força o perfil Dentista para evitar inclusão de Paciente aqui
//                model.Perfil = Perfil.Dentista;

//                bd.Usuarios.Add(model);
//                bd.SaveChanges();

//                // Redireciona para criar o Dentista associado
//                return RedirectToAction("Incluir", "Dentistas", new { area = "Admin", idusuario = model.Id });
//            }

//            return View(model);
//        }

//        [HttpGet]
//        public IActionResult Alterar(int id)
//        {
//            bd = new BancoDados();
//            var usuario = bd.Usuarios.FirstOrDefault(u => u.Id == id);
//            if (usuario == null)
//            {
//                return NotFound();
//            }

//            return View(usuario);
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public IActionResult Alterar(Usuario model)
//        {
//            if (ModelState.IsValid)
//            {
//                bd = new BancoDados();
//                bd.Usuarios.Update(model);
//                bd.SaveChanges();
//                return RedirectToAction("Index");
//            }

//            return View(model);
//        }

//        [HttpGet]
//        public IActionResult Exibir(int id)
//        {
//            bd = new BancoDados();
//            var usuario = bd.Usuarios.FirstOrDefault(u => u.Id == id);
//            if (usuario == null)
//            {
//                return NotFound();
//            }

//            return View(usuario);
//        }

//        [HttpGet]
//        public IActionResult Excluir(int id)
//        {
//            bd = new BancoDados();
//            var usuario = bd.Usuarios.FirstOrDefault(u => u.Id == id);
//            if (usuario == null)
//            {
//                return NotFound();
//            }

//            return View(usuario);
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public IActionResult Excluir(Usuario model)
//        {
//            bd = new BancoDados();
//            var usuario = bd.Usuarios.FirstOrDefault(u => u.Id == model.Id);
//            if (usuario != null)
//            {
//                bd.Usuarios.Remove(usuario);
//                bd.SaveChanges();
//                return RedirectToAction("Index");
//            }

//            return View(model);
//        }
//    }
//}
