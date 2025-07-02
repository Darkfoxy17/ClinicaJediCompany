using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using WebAppChamadosTI.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using WebAppChamadosTI.Models;


namespace WebAppChamadosTI.Controllers
{
    public class ContaController : Controller
    {
        [HttpGet]
        public IActionResult Sair()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Registrar()
        {
            var viewModel = new RegistroViewModel
            {
                Perfil = Perfil.Paciente
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Registrar(RegistroViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                BancoDados bd = new BancoDados();

                var usuarioExistente = bd.Usuarios.FirstOrDefault(u => u.Email == viewModel.Email);
                if (usuarioExistente != null)
                {
                    ModelState.AddModelError("Email", "Essa conta já é cadastrada");
                    return View(viewModel);
                }

                var usuario = new Usuario
                {
                    Email = viewModel.Email,
                    Senha = viewModel.Senha,
                    Perfil = Perfil.Paciente
                };

                bd.Usuarios.Add(usuario);
                bd.SaveChanges();

                var paciente = new Paciente
                {
                    UsuarioId = usuario.Id,
                    Nome = viewModel.Nome,
                    Telefone = viewModel.Telefone,
                    DataNascimento = viewModel.DataNascimento,
                    Endereco = viewModel.Endereco
                };

                bd.Pacientes.Add(paciente);
                bd.SaveChanges();

                return RedirectToAction("Entrar");
            }
            return View(viewModel);
        }


        [HttpGet]
        public IActionResult Entrar()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Entrar(ContaViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                BancoDados bd = new BancoDados();
                var usuario = bd.Usuarios.FirstOrDefault(usuario => usuario.Email == viewModel.Email && usuario.Senha == viewModel.Senha);

                if (usuario != null)
                {
                    int id;
                    string nome;

                    if (usuario.Perfil == Models.Perfil.Paciente)
                    {
                        var paciente = bd.Pacientes.FirstOrDefault(c => c.UsuarioId == usuario.Id);
                        id = paciente.Id;
                        nome = paciente.Nome;
                    }
                    else
                    {
                        var dentista = bd.Dentistas.FirstOrDefault(t => t.UsuarioId == usuario.Id);
                        id = dentista.Id;
                        nome = dentista.Nome;
                    }

                    List<Claim> dadosAcesso = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, nome),
                        new Claim(ClaimTypes.Email, usuario.Email),
                        new Claim(ClaimTypes.NameIdentifier, id.ToString()),
                        new Claim(ClaimTypes.Role, usuario.Perfil.ToString())
                    };

                    ClaimsIdentity identidadeAcesso = new ClaimsIdentity(dadosAcesso, CookieAuthenticationDefaults.AuthenticationScheme);

                    AuthenticationProperties cookieAutenticacao = new AuthenticationProperties
                    {
                        AllowRefresh = true,
                        IssuedUtc = DateTime.UtcNow,
                        ExpiresUtc = DateTime.UtcNow.AddMinutes(30),
                        RedirectUri = @"/conta/entrar"
                    };

                    ClaimsPrincipal autorizacaoAcesso = new ClaimsPrincipal(identidadeAcesso);
                    HttpContext.SignInAsync(autorizacaoAcesso, cookieAutenticacao);

                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                else
                {
                    ModelState.AddModelError("Senha", "Usuário ou senha inválidos");
                }
            }

            return View(viewModel);
        }
    }
}
