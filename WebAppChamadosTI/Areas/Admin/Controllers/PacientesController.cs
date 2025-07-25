using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebAppChamadosTI.Data;
using WebAppChamadosTI.Models;

namespace WebAppChamadosTI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PacientesController : Controller
    {
        BancoDados bd;

        [HttpGet]
        [HttpGet]
        public IActionResult Index()
        {
            bd = new BancoDados();

            // Se for Dentista, acesso negado
            if (User.IsInRole("Dentista"))
                return RedirectToAction("AcessoNegado", "Home");

            // Se for Paciente, só pode ver o próprio registro
            if (User.IsInRole("Paciente"))
            {
                int idPacienteLogado = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var paciente = bd.Pacientes.Include(p => p.Usuario)
                                           .FirstOrDefault(p => p.Id == idPacienteLogado);
                if (paciente == null) return NotFound();

                ViewBag.MostrarBusca = false;
                return View(new List<Paciente> { paciente });
            }

            // Para Atendente ou Administrador
            var listaPacientes = bd.Pacientes.Include(c => c.Usuario).ToList();
            ViewBag.MostrarBusca = User.IsInRole("Atendente");

            return View(listaPacientes);
        }


        [HttpPost]
        public IActionResult Index(string busca)
        {
            bd = new BancoDados();

            if (User.IsInRole("Dentista"))
                return RedirectToAction("AcessoNegado", "Home");

            // Paciente só vê a si mesmo
            if (User.IsInRole("Paciente"))
            {
                int idPacienteLogado = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var paciente = bd.Pacientes.Include(p => p.Usuario)
                                           .FirstOrDefault(p => p.Id == idPacienteLogado);
                if (paciente == null) return NotFound();

                ViewBag.MostrarBusca = false;
                return View(new List<Paciente> { paciente });
            }

            var lista = bd.Pacientes.Include(c => c.Usuario).ToList();

            if (!string.IsNullOrWhiteSpace(busca))
            {
                lista = lista
                    .Where(c => c.Nome.Contains(busca, StringComparison.OrdinalIgnoreCase) ||
                                c.Usuario.Email.Contains(busca, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            ViewBag.MostrarBusca = User.IsInRole("Atendente");

            return View(lista);
        }


        [HttpGet]
        public IActionResult Alterar(int id)
        {
            if (User.IsInRole("Dentista"))
                return RedirectToAction("AcessoNegado", "Home");

            if (!(User.IsInRole("Paciente") || User.IsInRole("Atendente") || User.IsInRole("Administrador")))
                return RedirectToAction("AcessoNegado", "Home");

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
            if (!(User.IsInRole("Paciente") || User.IsInRole("Atendente") || User.IsInRole("Administrador")))
                return RedirectToAction("AcessoNegado", "Home");

            bd = new BancoDados();

            var pacienteExistente = bd.Pacientes.FirstOrDefault(p => p.Id == model.Id);
            if (pacienteExistente == null) return NotFound();

            pacienteExistente.Nome = model.Nome;
            pacienteExistente.Telefone = model.Telefone;
            pacienteExistente.Endereco = model.Endereco;
            pacienteExistente.DataNascimento = model.DataNascimento;

            bd.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Exibir(int id)
        {
            if (User.IsInRole("Dentista"))
                return RedirectToAction("AcessoNegado", "Home");

            bd = new BancoDados();
            var paciente = bd.Pacientes.Include(c => c.Usuario).FirstOrDefault(c => c.Id == id);
            return View(paciente);
        }

        [HttpGet]
        public IActionResult Incluir()
        {
            return RedirectToAction("AcessoNegado", "Home");
        }

        [HttpGet]
        public IActionResult Excluir(int id)
        {
            if (!(User.IsInRole("Atendente") || User.IsInRole("Administrador")))
                return RedirectToAction("AcessoNegado", "Home");

            bd = new BancoDados();
            var paciente = bd.Pacientes.Include(c => c.Usuario).FirstOrDefault(c => c.Id == id);
            if (paciente == null) return NotFound();

            return View(paciente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Excluir(Paciente model)
        {
            if (!(User.IsInRole("Atendente") || User.IsInRole("Administrador")))
                return RedirectToAction("AcessoNegado", "Home");

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
