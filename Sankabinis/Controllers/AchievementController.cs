using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sankabinis.Data;
using Sankabinis.Models;

namespace Sankabinis.Controllers
{
    public class AchievementController : Controller
    {
        private readonly SankabinisContext _context;
        public AchievementController(SankabinisContext context)
        {
            _context = context;
        }

        public IActionResult ShowAchievementList()
        {
            List<Achievement> achievements = _context.Achievement.ToList();
            return Json(achievements);
        }

    }
}
