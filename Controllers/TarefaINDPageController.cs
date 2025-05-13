using Microsoft.AspNetCore.Mvc;
using WebApplication2.Data;

namespace WebApplication2.Controllers
{
    public class TarefaINDPageController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TarefaINDPageController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(decimal projetoId)
        {
            var projeto = await _context.Projetos.FindAsync(projetoId);
            if (projeto == null) return NotFound();

            ViewBag.ProjetoId = projeto.IdProjeto;
            ViewBag.NomeProjeto = projeto.NomeProjeto;

            return View("~/Views/Home/TarefasIND.cshtml"); 
        }
    }
}