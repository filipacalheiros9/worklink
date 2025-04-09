using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using WebApplication2.Entities;

namespace WebApplication2.Controllers;

public class PerfilController : Controller
{
    private readonly ApplicationDbContext _context;

    public PerfilController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return RedirectToAction("Login", "Home");
        }

        var utilizador = await _context.Utilizadores.FindAsync(userId);
        if (utilizador == null)
        {
            return NotFound();
        }

        return View(utilizador);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Utilizador model)
    {
        if (ModelState.IsValid)
        {
            var utilizador = await _context.Utilizadores.FindAsync(model.IdUtilizador);
            if (utilizador == null)
            {
                return NotFound();
            }

            utilizador.Nome = model.Nome;
            utilizador.Username = model.Username;
            utilizador.Password = model.Password;

            _context.Utilizadores.Update(utilizador);
            await _context.SaveChangesAsync();

            ViewBag.Sucesso = "Perfil atualizado com sucesso!";
            return View("Index", utilizador);
        }

        return View("Index", model);
    }
}