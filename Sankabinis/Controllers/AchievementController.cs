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
            var loggedInUserId = HttpContext.Session.GetInt32("UserId");
            List<Achievement> achievements = _context.Achievement
                                                     .Where(a => a.UserId == loggedInUserId.Value)
                                                     .ToList();
            return Json(achievements);
        }

    }
}
