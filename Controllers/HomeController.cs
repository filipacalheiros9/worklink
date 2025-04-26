using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using WebApplication2.Entities;
using WebApplication2.Models;
using WebApplication2.Services;
using WebApplication2.Factories; 
namespace WebApplication2.Controllers
{
    public partial class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUtilizadorService _utilizadorService;

        public HomeController(ILogger<HomeController> logger, IUtilizadorService utilizadorService)
        {
            _logger = logger;
            _utilizadorService = utilizadorService;
        }

        public IActionResult Index() => View();
        public IActionResult Contato() => View();
        public IActionResult Register() => View();
        public IActionResult HomePageLogin() => View();
        public IActionResult AsMinhasEquipas() => View();
        public IActionResult CriarProjeto() => View();
        public IActionResult Defenicoes() => View();
        public IActionResult Tarefas() => View();
        public IActionResult adminpage() => View();

        [HttpGet]
        public IActionResult Login() => View();

        
        [HttpPost]
        public async Task<IActionResult> Login(string Username, string Password)
        {
            var utilizador = await _utilizadorService.ValidateLoginAsync(Username, Password);

            if (utilizador != null)
            {
                // ✅ CRIA AS CLAIMS
                var claims = new List<Claim>
                {
                    new Claim("id", utilizador.IdUtilizador.ToString()),
                    new Claim(ClaimTypes.Name, utilizador.Username),
                    new Claim(ClaimTypes.Role, utilizador.cargo)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                // ✅ SIGN IN COM COOKIES
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                // Se quiseres ainda usar a sessão, podes manter
                HttpContext.Session.SetString("IdUtilizador", utilizador.IdUtilizador.ToString());
                HttpContext.Session.SetString("LoggedIn", "true");
                HttpContext.Session.SetString("Cargo", utilizador.cargo);

                // Redireciona com base no cargo do utilizador
                if (utilizador.cargo == "admin")
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

            if (string.IsNullOrEmpty(idStr) || !decimal.TryParse(idStr, out var id))
                return RedirectToAction("Login", "Home");

            var utilizador = await _utilizadorService.GetUtilizadorByIdAsync(id);

            if (utilizador == null)
                return RedirectToAction("Login", "Home");

            return View(utilizador);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
