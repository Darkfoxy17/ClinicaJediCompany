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
        public IActionResult Registrar(string perfil)
        {
            RegistroViewModel viewModel = new RegistroViewModel();

            // Verificar o tipo de registro
            viewModel.Perfil = perfil == "Tecnico" ? Perfil.Tecnico : Perfil.Cliente;

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Registrar(RegistroViewModel viewModel, IFormFile? arquivo)
        {
            if (ModelState.IsValid)
            {
                BancoDados bd = new BancoDados();

                // Verifica se já existe a conta do usuário
                var usuario = bd.Usuarios.FirstOrDefault(u => u.Email == viewModel.Email);
                if (usuario != null)
                {
                    ModelState.AddModelError("Email", "Essa conta já é cadastrada");
                    return View(viewModel);
                }

                // Inclui o novo usuário
                usuario = new Usuario
                {
                    Email = viewModel.Email,
                    Senha = viewModel.Senha,
                    Perfil = viewModel.Perfil
                };

                // Processamento do arquivo de imagem
                if (arquivo != null && arquivo.Length > 0)
                {
                    string nomeArquivo = Guid.NewGuid().ToString() + Path.GetExtension(arquivo.FileName);
                    string caminho = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", nomeArquivo);
                    Directory.CreateDirectory(Path.GetDirectoryName(caminho)!);

                    using (var stream = new FileStream(caminho, FileMode.Create))
                    {
                        arquivo.CopyTo(stream);
                    }

                    usuario.Arquivo = nomeArquivo;
                }

                bd.Usuarios.Add(usuario);
                bd.SaveChanges();

                // Cliente
                if (viewModel.Perfil == Perfil.Cliente)
                {
                    if (string.IsNullOrWhiteSpace(viewModel.Profissao))
                    {
                        ModelState.AddModelError("Profissao", "O campo profissão é obrigatório");
                        return View(viewModel);
                    }

                    Cliente cliente = new Cliente
                    {
                        UsuarioId = usuario.Id,
                        Nome = viewModel.Nome,
                        Profissao = viewModel.Profissao
                    };
                    bd.Clientes.Add(cliente);
                    bd.SaveChanges();
                }

                // Técnico
                if (viewModel.Perfil == Perfil.Tecnico)
                {
                    if (string.IsNullOrWhiteSpace(viewModel.Especialidade))
                    {
                        ModelState.AddModelError("Especialidade", "O campo especialidade é obrigatório");
                        return View(viewModel);
                    }

                    Tecnico tecnico = new Tecnico
                    {
                        UsuarioId = usuario.Id,
                        Nome = viewModel.Nome,
                        Especialidade = viewModel.Especialidade
                    };
                    bd.Tecnicos.Add(tecnico);
                    bd.SaveChanges();
                }

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

                    if (usuario.Perfil == Models.Perfil.Cliente)
                    {
                        var cliente = bd.Clientes.FirstOrDefault(c => c.UsuarioId == usuario.Id);
                        id = cliente.Id;
                        nome = cliente.Nome;
                    }
                    else
                    {
                        var tecnico = bd.Tecnicos.FirstOrDefault(t => t.UsuarioId == usuario.Id);
                        id = tecnico.Id;
                        nome = tecnico.Nome;
                    }

                    // Autenticar o usuário
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
