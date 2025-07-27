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
                    .Include(d => d.Especializacao) // ← ADICIONADO
                    .FirstOrDefault(d => d.UsuarioId == usuarioLogado.Id);

                ViewBag.NomeUsuario = dentista?.Nome ?? usuarioLogado.Email;

                if (dentista == null)
                    return View(new List<Dentista>());

                return View(new List<Dentista> { dentista });
            }

            var listaDentistas = bd.Dentistas
                .Include(u => u.Usuario)
                .Include(d => d.Especializacao) // ← ADICIONADO
                .Where(d => d.Usuario.Perfil != Perfil.Atendente)
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
                    .Include(d => d.Especializacao)
                    .FirstOrDefault(d => d.UsuarioId == usuarioLogado.Id);

                ViewBag.NomeUsuario = dentista?.Nome ?? usuarioLogado.Email;

                if (dentista == null)
                    return View(new List<Dentista>());

                return View(new List<Dentista> { dentista });
            }

            var query = bd.Dentistas
                .Include(u => u.Usuario)
                .Include(d => d.Especializacao)
                .Where(d => d.Usuario.Perfil != Perfil.Atendente)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(busca))
            {
                query = query.Where(t =>
                    t.Nome.Contains(busca) ||
                    t.Especializacao.Nome.Contains(busca));
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

            bd = new BancoDados();

            var model = new DentistaViewModel
            {
                EspecializacoesDisponiveis = bd.Especializacoes
                    .Select(e => new SelectListItem
                    {
                        Value = e.Id.ToString(),
                        Text = e.Nome
                    })
                    .ToList()
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Incluir(DentistaViewModel model)
        {
            if (!User.IsInRole("Atendente"))
                return RedirectToAction("AcessoNegado", "Home", new { area = "" });

            using var bd = new BancoDados();

            if (!ModelState.IsValid)
            {
                // Recarrega especializações para o dropdown
                model.EspecializacoesDisponiveis = bd.Especializacoes
                    .Select(e => new SelectListItem
                    {
                        Value = e.Id.ToString(),
                        Text = e.Nome
                    })
                    .ToList();

                return View(model);
            }
            var usuario = new Usuario
            {
                Email = model.Email,
                Senha = model.Senha,
                Perfil = Perfil.Dentista

            };
            bd.Usuarios.Add(usuario);
            bd.SaveChanges();

            var dentista = new Dentista
            {
                Nome = model.Nome,
                Telefone = model.Telefone,
                DataNascimento = model.DataNascimento,
                Endereco = model.Endereco,
                UsuarioId = usuario.Id,
                EspecializacaoId = model.EspecializacaoId
            };
            bd.Dentistas.Add(dentista);
            bd.SaveChanges();

            var procedimentos = bd.EspecializacoesProcedimentos
                .Where(ep => ep.EspecializacaoId == model.EspecializacaoId)
                .Select(ep => ep.ProcedimentoId)
                .ToList();

            // 4. Associa os procedimentos ao dentista
            foreach (var procedimentoId in procedimentos)
            {
                bd.DentistaProcedimentos.Add(new DentistaProcedimento
                {
                    DentistaId = dentista.Id,
                    ProcedimentoId = procedimentoId
                });
            }
            bd.SaveChanges();

            return RedirectToAction("Index");
        }




        public IActionResult Alterar(int id)
        {
            bd = new BancoDados();

            var dentista = bd.Dentistas
                .Include(t => t.Usuario)
                .Include(t => t.DentistaProcedimentos)
                .FirstOrDefault(t => t.Id == id);

            if (dentista == null || dentista.Usuario.Perfil == Perfil.Atendente)
                return NotFound();

            var usuarioLogado = ObterUsuarioLogado();

            if (!(User.IsInRole("Dentista") || User.IsInRole("Atendente")))
                return RedirectToAction("AcessoNegado", "Home", new { area = "" });

            if (usuarioLogado.Perfil == Perfil.Dentista && dentista.UsuarioId != usuarioLogado.Id)
                return RedirectToAction("AcessoNegado", "Home", new { area = "" });

            var viewModel = new DentistaEditarViewModel
            {
                Id = dentista.Id,
                Nome = dentista.Nome,
                Telefone = dentista.Telefone,
                DataNascimento = dentista.DataNascimento,
                Endereco = dentista.Endereco,
                EspecializacaoId = dentista.EspecializacaoId,
                ProcedimentosIds = dentista.DentistaProcedimentos.Select(dp => dp.ProcedimentoId).ToList(),
                ProcedimentosDisponiveis = ObterProcedimentos(),
                EspecializacoesDisponiveis = ObterEspecializacoes()
            };

            return View(viewModel);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Alterar(DentistaEditarViewModel model)
        {
            bd = new BancoDados();
            var usuarioLogado = ObterUsuarioLogado();

            var dentista = bd.Dentistas
                .Include(d => d.DentistaProcedimentos)
                .FirstOrDefault(t => t.Id == model.Id);

            if (dentista == null || bd.Usuarios.FirstOrDefault(u => u.Id == dentista.UsuarioId)?.Perfil == Perfil.Atendente)
                return NotFound();

            if (!(User.IsInRole("Dentista") || User.IsInRole("Atendente")))
                return RedirectToAction("AcessoNegado", "Home", new { area = "" });

            if (usuarioLogado.Perfil == Perfil.Dentista && dentista.UsuarioId != usuarioLogado.Id)
                return RedirectToAction("AcessoNegado", "Home", new { area = "" });

            if (!ModelState.IsValid)
            {
                model.ProcedimentosDisponiveis = ObterProcedimentos();
                model.EspecializacoesDisponiveis = ObterEspecializacoes();
                return View(model);
            }

            if (usuarioLogado.Perfil == Perfil.Atendente)
            {
                var outroDentista = bd.Dentistas.FirstOrDefault(t => t.UsuarioId == dentista.UsuarioId && t.Id != dentista.Id);
                var paciente = bd.Pacientes.FirstOrDefault(c => c.UsuarioId == dentista.UsuarioId);

                if (outroDentista != null || paciente != null)
                {
                    ModelState.AddModelError("UsuarioId", "Este e-mail já está vinculado a outro cadastro!");

                    model.ProcedimentosDisponiveis = ObterProcedimentos();
                    model.EspecializacoesDisponiveis = ObterEspecializacoes();
                    return View(model);
                }
            }

            dentista.Nome = model.Nome;
            dentista.Telefone = model.Telefone;
            dentista.DataNascimento = model.DataNascimento;
            dentista.Endereco = model.Endereco;
            dentista.EspecializacaoId = model.EspecializacaoId;

            dentista.DentistaProcedimentos.Clear();
            foreach (var procedimentoId in model.ProcedimentosIds)
            {
                dentista.DentistaProcedimentos.Add(new DentistaProcedimento
                {
                    DentistaId = dentista.Id,
                    ProcedimentoId = procedimentoId
                });
            }

            bd.SaveChanges();
            return RedirectToAction("Index");
        }





        private List<SelectListItem> ObterEspecializacoes()
        {
            return bd.Especializacoes.Select(e => new SelectListItem
            {
                Value = e.Id.ToString(),
                Text = e.Nome
            }).ToList();
        }

        private List<SelectListItem> ObterProcedimentos()
        {
            return bd.Procedimentos.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.Nome
            }).ToList();
        }


        [HttpGet]
        public IActionResult Exibir(int id)
        {
            bd = new BancoDados();
            var dentista = bd.Dentistas
                .Include(t => t.Usuario)
                .Include(t => t.Especializacao) // 👈 ADICIONAR ISSO!
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
            var dentista = bd.Dentistas
                .Include(d => d.Usuario)
                .Include(d => d.DentistaProcedimentos)
                .FirstOrDefault(t => t.Id == model.Id);

            var usuarioLogado = ObterUsuarioLogado();

            if (dentista == null || dentista.Usuario.Perfil == Perfil.Atendente)
                return NotFound();

            if (!User.IsInRole("Atendente"))
                return RedirectToAction("AcessoNegado", "Home", new { area = "" });

            // Exclui os registros de DentistaProcedimentos
            bd.DentistaProcedimentos.RemoveRange(dentista.DentistaProcedimentos);

            // Depois, exclui o dentista
            bd.Dentistas.Remove(dentista);
            bd.SaveChanges();

            return RedirectToAction("Index");
        }



    }
}
