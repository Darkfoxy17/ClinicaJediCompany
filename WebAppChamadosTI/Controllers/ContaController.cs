using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebAppChamadosTI.Data;
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
        public async Task<IActionResult> Entrar(ContaViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                BancoDados bd = new BancoDados();
                var usuario = bd.Usuarios.FirstOrDefault(u =>
                    u.Email == viewModel.Email && u.Senha == viewModel.Senha);

                if (usuario != null)
                {
                    int id = 0;
                    string nome = string.Empty;
                    bool cadastroValido = false;

                    switch (usuario.Perfil)
                    {
                        case Perfil.Paciente:
                            var paciente = bd.Pacientes.FirstOrDefault(c => c.UsuarioId == usuario.Id);
                            if (paciente != null)
                            {
                                id = paciente.Id;
                                nome = paciente.Nome;
                                cadastroValido = true;
                            }
                            break;

                        case Perfil.Dentista:
                        case Perfil.Atendente: // Atendente também está na tabela Dentistas
                            var dentista = bd.Dentistas.FirstOrDefault(d => d.UsuarioId == usuario.Id);
                            if (dentista != null)
                            {
                                id = dentista.Id;
                                nome = dentista.Nome;
                                cadastroValido = true;
                            }
                            break;
                    }

                    if (!cadastroValido)
                    {
                        ModelState.AddModelError("", "Cadastro não encontrado para o perfil informado.");
                        return View(viewModel);
                    }

                    // Autenticação
                    List<Claim> dadosAcesso = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.Email),
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
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, autorizacaoAcesso, cookieAutenticacao);

                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }

                ModelState.AddModelError("Senha", "Usuário ou senha inválidos");
            }

            return View(viewModel);
        }



        [Authorize]
    [HttpGet]
    public IActionResult AlterarSenha()
    {
        return View();
    }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AlterarSenha(AlterarSenhaViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                BancoDados bd = new BancoDados();

                string emailUsuario = User.Identity.Name;

                var usuario = bd.Usuarios.FirstOrDefault(u => u.Email == emailUsuario);

                if (usuario == null)
                {
                    return Unauthorized();
                }

                if (usuario.Senha != viewModel.SenhaAtual)
                {
                    ModelState.AddModelError("SenhaAtual", "A senha atual está incorreta.");
                    return View(viewModel);
                }

                usuario.Senha = viewModel.NovaSenha;
                bd.SaveChanges();

                // 🧼 Encerra a sessão atual
                await HttpContext.SignOutAsync();

                // 🔁 Redireciona para login
                return RedirectToAction("Entrar", "Conta");
            }

            return View(viewModel);
        }




    }
}
