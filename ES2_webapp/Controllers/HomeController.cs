using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using ES2_webapp.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using WebApplication2.Entities;
using WebApplication2.Models;
using WebApplication2.Services;
using WebApplication2.Factories;
using Microsoft.EntityFrameworkCore;

namespace WebApplication2.Controllers
{
    public partial class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUtilizadorService _utilizadorService;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, IUtilizadorService utilizadorService, ApplicationDbContext context)
        {
            _logger = logger;
            _utilizadorService = utilizadorService;
            _context = context;
        }

        public IActionResult Index() => View();
        public IActionResult Contato() => View();
        public IActionResult Register() => View();
        public IActionResult HomePageLogin() => View();
        public IActionResult AsMinhasEquipas() => View();
        public IActionResult CriarProjeto() => View();
        public IActionResult Tarefas() => View();

        public IActionResult adminpage()
        {
            var cargoIdStr = HttpContext.Session.GetString("CargoId");
            if (cargoIdStr == null || cargoIdStr != "1")
            {
                return Unauthorized(); // ou RedirectToAction("HomePageLogin");
            }

            return View();
        }

        public IActionResult TarefasIND() => View();
        public IActionResult Convites() => View();
        public IActionResult Login() => View();
        public IActionResult ProjetosEquipa() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string Username, string Password)
        {
            var utilizador = await _utilizadorService.ValidateLoginAsync(Username, Password);

            if (utilizador != null)
            {
                var cargoNome = utilizador.Cargo?.Nome ?? "Utilizador";

                var claims = new List<Claim>
                {
                    new Claim("id", utilizador.IdUtilizador.ToString()),
                    new Claim(ClaimTypes.Name, utilizador.Username),
                    new Claim(ClaimTypes.Role, cargoNome)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                HttpContext.Session.SetString("IdUtilizador", utilizador.IdUtilizador.ToString());
                HttpContext.Session.SetString("LoggedIn", "true");
                HttpContext.Session.SetString("Cargo", cargoNome);
                HttpContext.Session.SetString("CargoId", utilizador.CargoId.ToString()); // <-- novo

                if (utilizador.CargoId == 1)
                {
                    return RedirectToAction("adminpage", "Home");
                }
                else
                {
                    return RedirectToAction("HomePageLogin", "Home");
                }
            }

            ViewBag.ErrorMessage = "Credenciais inválidas.";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(Utilizador utilizador)
        {
            if (ModelState.IsValid)
            {
                var totalUtilizadores = await _utilizadorService.CountUtilizadoresAsync();

                var novoUtilizador = UtilizadorFactory.CriarNovo(
                    utilizador.Nome,
                    utilizador.Username,
                    utilizador.Password,
                    totalUtilizadores
                );

                await _utilizadorService.RegisterUtilizadorAsync(novoUtilizador);

                return RedirectToAction("Index");
            }

            return View(utilizador);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Perfil()
        {
            var idStr = HttpContext.Session.GetString("IdUtilizador");

            if (string.IsNullOrEmpty(idStr) || !int.TryParse(idStr, out var id))
                return RedirectToAction("Login", "Home");

            var utilizador = await _utilizadorService.GetUtilizadorByIdAsync(id);

            if (utilizador == null)
                return RedirectToAction("Login", "Home");

            // Buscar projetos do utilizador (pessoais e de equipa)
            var projetosDoUtilizador = await _context.Projetos
                .Where(p =>
                    p.IdUtilizador == id
                    || (p.EquipaId != null &&
                        _context.EquipaUtilizadores.Any(eu => eu.EquipaId == p.EquipaId && eu.UtilizadorId == id))
                )
                .OrderBy(p => p.NomeProjeto)
                .ToListAsync();
            ViewBag.ProjetosDoUser = projetosDoUtilizador;

            return View(utilizador);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
