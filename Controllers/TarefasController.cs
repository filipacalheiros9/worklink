using Microsoft.AspNetCore.Mvc;
using WebApplication2.Data;

namespace WebApplication2.Controllers
{
    public class TarefasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TarefasController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int projetoId)
        {
            var projeto = _context.Projetos.FirstOrDefault(p => p.IdProjeto == projetoId);
            if (projeto == null) return NotFound();

            ViewBag.ProjetoId = projeto.IdProjeto;
            ViewBag.NomeProjeto = projeto.NomeProjeto;

            return View("~/Views/Home/Tarefas.cshtml");
        }
    }
}