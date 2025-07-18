using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebAppChamadosTI.Data;
using WebAppChamadosTI.Models;

namespace WebAppChamadosTI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DentistasController : Controller
    {
        BancoDados bd;

        public DentistasController() { }

        private Usuario ObterUsuarioLogado()
        {
            bd = new BancoDados();
            return bd.Usuarios.FirstOrDefault(u => u.Email == User.Identity.Name);
        }

        [HttpGet]
        public IActionResult Index()
        {
            bd = new BancoDados();
            var usuarioLogado = ObterUsuarioLogado();

            if (usuarioLogado == null || !(User.IsInRole("Dentista") || User.IsInRole("Atendente")))
                return RedirectToAction("AcessoNegado", "Home", new { area = "" });

            ViewBag.ExibirBusca = usuarioLogado.Perfil == Perfil.Atendente;

            if (usuarioLogado.Perfil == Perfil.Dentista)
            {
                var dentista = bd.Dentistas
                    .Include(u => u.Usuario)
                    .FirstOrDefault(d => d.UsuarioId == usuarioLogado.Id);

                ViewBag.NomeUsuario = dentista?.Nome ?? usuarioLogado.Email;

                if (dentista == null)
                    return View(new List<Dentista>());

                return View(new List<Dentista> { dentista });
            }

            var listaDentistas = bd.Dentistas
                .Include(u => u.Usuario)
                .Where(d => d.Usuario.Perfil != Perfil.Atendente) // Oculta os que têm perfil Atendente
                .ToList();

            ViewBag.NomeUsuario = usuarioLogado.Email;

            return View(listaDentistas);
        }

        [HttpPost]
        public IActionResult Index(string busca)
        {
            bd = new BancoDados();
            var usuarioLogado = ObterUsuarioLogado();

            if (usuarioLogado == null || !(User.IsInRole("Dentista") || User.IsInRole("Atendente")))
                return RedirectToAction("AcessoNegado", "Home", new { area = "" });

            ViewBag.ExibirBusca = usuarioLogado.Perfil == Perfil.Atendente;

            if (usuarioLogado.Perfil == Perfil.Dentista)
            {
                var dentista = bd.Dentistas
                    .Include(u => u.Usuario)
                    .FirstOrDefault(d => d.UsuarioId == usuarioLogado.Id);

                ViewBag.NomeUsuario = dentista?.Nome ?? usuarioLogado.Email;

                if (dentista == null)
                    return View(new List<Dentista>());

                return View(new List<Dentista> { dentista });
            }

            var query = bd.Dentistas
                .Include(u => u.Usuario)
                .Where(d => d.Usuario.Perfil != Perfil.Atendente)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(busca))
            {
                query = query.Where(t =>
                    t.Nome.Contains(busca) ||
                    t.Usuario.Email.Contains(busca));
            }

            var listaDentistas = query.ToList();

            ViewBag.NomeUsuario = usuarioLogado.Email;

            return View(listaDentistas);
        }

        [HttpGet]
        public IActionResult Incluir(int idusuario)
        {
            var usuarioLogado = ObterUsuarioLogado();
            if (!User.IsInRole("Atendente"))
                return RedirectToAction("AcessoNegado", "Home", new { area = "" });

            return View(new DentistaViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Incluir(DentistaViewModel model)
        {
            bd = new BancoDados();
            var usuarioLogado = ObterUsuarioLogado();

            if (!User.IsInRole("Atendente"))
                return RedirectToAction("AcessoNegado", "Home", new { area = "" });

            if (ModelState.IsValid)
            {
                var usuarioExistente = bd.Usuarios.FirstOrDefault(u => u.Email == model.Email);
                if (usuarioExistente != null)
                {
                    ModelState.AddModelError("Email", "Este e-mail já está em uso.");
                    return View(model);
                }

                var novoUsuario = new Usuario
                {
                    Email = model.Email,
                    Senha = model.Senha,
                    Perfil = Perfil.Dentista
                };
                bd.Usuarios.Add(novoUsuario);
                bd.SaveChanges();

                var novoDentista = new Dentista
                {
                    Nome = model.Nome,
                    Telefone = model.Telefone,
                    DataNascimento = model.DataNascimento,
                    Endereco = model.Endereco,
                    Especialidade = model.Especialidade,
                    UsuarioId = novoUsuario.Id
                };
                bd.Dentistas.Add(novoDentista);
                bd.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Alterar(int id)
        {
            bd = new BancoDados();
            var dentista = bd.Dentistas
                .Include(t => t.Usuario)
                .FirstOrDefault(t => t.Id == id);

            if (dentista == null || dentista.Usuario.Perfil == Perfil.Atendente)
                return NotFound();

            var usuarioLogado = ObterUsuarioLogado();

            if (!(User.IsInRole("Dentista") || User.IsInRole("Atendente")))
                return RedirectToAction("AcessoNegado", "Home", new { area = "" });

            if (usuarioLogado.Perfil == Perfil.Dentista && dentista.UsuarioId != usuarioLogado.Id)
                return RedirectToAction("AcessoNegado", "Home", new { area = "" });

            if (usuarioLogado.Perfil == Perfil.Atendente)
            {
                var listaUsuarios = bd.Usuarios
                    .Where(u => u.Perfil != Perfil.Atendente)
                    .ToList();
                ViewBag.Usuarios = new SelectList(listaUsuarios, "Id", "Email", dentista.UsuarioId);
            }

            return View(dentista);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Alterar(Dentista model)
        {
            bd = new BancoDados();
            var usuarioLogado = ObterUsuarioLogado();
            var dentista = bd.Dentistas.FirstOrDefault(t => t.Id == model.Id);

            if (dentista == null || bd.Usuarios.FirstOrDefault(u => u.Id == dentista.UsuarioId)?.Perfil == Perfil.Atendente)
                return NotFound();

            if (!(User.IsInRole("Dentista") || User.IsInRole("Atendente")))
                return RedirectToAction("AcessoNegado", "Home", new { area = "" });

            if (usuarioLogado.Perfil == Perfil.Dentista && dentista.UsuarioId != usuarioLogado.Id)
                return RedirectToAction("AcessoNegado", "Home", new { area = "" });

            if (ModelState.IsValid)
            {
                if (usuarioLogado.Perfil == Perfil.Atendente)
                {
                    var outroDentista = bd.Dentistas
                        .FirstOrDefault(t => t.UsuarioId == model.UsuarioId && t.Id != model.Id);
                    var paciente = bd.Pacientes
                        .FirstOrDefault(c => c.UsuarioId == model.UsuarioId);

                    if (outroDentista != null || paciente != null)
                    {
                        ModelState.AddModelError("UsuarioId", "Este e-mail já está vinculado a outro cadastro!");
                        var listaUsuarios = bd.Usuarios
                            .Where(u => u.Perfil != Perfil.Atendente)
                            .ToList();
                        ViewBag.Usuarios = new SelectList(listaUsuarios, "Id", "Email", model.UsuarioId);
                        return View(model);
                    }

                    dentista.UsuarioId = model.UsuarioId;
                }

                dentista.Nome = model.Nome;
                dentista.Especialidade = model.Especialidade;
                dentista.Telefone = model.Telefone;
                dentista.DataNascimento = model.DataNascimento;
                dentista.Endereco = model.Endereco;

                bd.SaveChanges();
                return RedirectToAction("Index");
            }

            if (usuarioLogado.Perfil == Perfil.Atendente)
            {
                var listaUsuarios = bd.Usuarios
                    .Where(u => u.Perfil != Perfil.Atendente)
                    .ToList();
                ViewBag.Usuarios = new SelectList(listaUsuarios, "Id", "Email", model.UsuarioId);
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Exibir(int id)
        {
            bd = new BancoDados();
            var dentista = bd.Dentistas
                .Include(t => t.Usuario)
                .FirstOrDefault(t => t.Id == id);

            if (dentista == null || dentista.Usuario.Perfil == Perfil.Atendente)
                return NotFound();

            var usuarioLogado = ObterUsuarioLogado();
            if (!(User.IsInRole("Dentista") || User.IsInRole("Atendente")))
                return RedirectToAction("AcessoNegado", "Home", new { area = "" });

            if (usuarioLogado.Perfil == Perfil.Dentista && dentista.UsuarioId != usuarioLogado.Id)
                return RedirectToAction("AcessoNegado", "Home", new { area = "" });

            return View(dentista);
        }

        [HttpGet]
        public IActionResult Excluir(int id)
        {
            bd = new BancoDados();
            var dentista = bd.Dentistas
                .Include(t => t.Usuario)
                .FirstOrDefault(t => t.Id == id);

            if (dentista == null || dentista.Usuario.Perfil == Perfil.Atendente)
                return NotFound();

            var usuarioLogado = ObterUsuarioLogado();
            if (!User.IsInRole("Atendente"))
                return RedirectToAction("AcessoNegado", "Home", new { area = "" });

            return View(dentista);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Excluir(Dentista model)
        {
            bd = new BancoDados();
            var dentista = bd.Dentistas.Include(d => d.Usuario).FirstOrDefault(t => t.Id == model.Id);
            var usuarioLogado = ObterUsuarioLogado();

            if (dentista == null || dentista.Usuario.Perfil == Perfil.Atendente)
                return NotFound();

            if (!User.IsInRole("Atendente"))
                return RedirectToAction("AcessoNegado", "Home", new { area = "" });

            bd.Dentistas.Remove(dentista);
            bd.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
