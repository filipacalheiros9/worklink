using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using WebApplication2.Entities;
using WebApplication2.Models;

namespace WebApplication2.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Contato()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    
    
    [HttpPost]
    public async Task<IActionResult> Login(string Username, string Password)
    {
        var utilizador = await _context.Utilizadores
            .FirstOrDefaultAsync(u => u.Username == Username && u.Password == Password);

        if (utilizador != null)
        {
            
            HttpContext.Session.SetString("IdUtilizador", utilizador.IdUtilizador.ToString());
            HttpContext.Session.SetString("LoggedIn", "true");

            return RedirectToAction("HomePageLogin", "Home"); 
        }

        ViewBag.ErrorMessage = "Credenciais inválidas.";
        return View();
    }


    public IActionResult Register()
    {
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Register(Utilizador utilizador)
    {
        if (ModelState.IsValid)
        {
            _context.Utilizadores.Add(utilizador);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        return View(utilizador);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    
    public IActionResult HomePageLogin()
    {
        return View();
    }
    
    public IActionResult Logout()
    {
        HttpContext.Session.Clear(); 
        return RedirectToAction("Index"); 
    }
    
    public async Task<IActionResult> Perfil()
    {
        var idStr = HttpContext.Session.GetString("IdUtilizador");

        if (string.IsNullOrEmpty(idStr))
            return RedirectToAction("Login", "Home");

        decimal id = decimal.Parse(idStr);

        var utilizador = await _context.Utilizadores
            .Where(u => u.IdUtilizador == id)
            .FirstOrDefaultAsync();

        if (utilizador == null)
            return RedirectToAction("Login", "Home");

        return View(utilizador);
    }



    public IActionResult AsMinhasEquipas()
    {
        return View();
    }

    public IActionResult CriarProjeto()
    {
        return View();
    }

    public IActionResult Defenicoes()
    {
        return View();
    }
}
