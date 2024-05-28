using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sankabinis.Data;
using Sankabinis.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Sankabinis.Controllers
{
    public class AchievementsController : Controller
    {
        private readonly SankabinisContext _context;

        public AchievementsController(SankabinisContext context)
        {
            _context = context;
        }

        // GET: Achievements
        public async Task<IActionResult> Index()
        {
            var achievements = await _context.Achievement.ToListAsync();
            return View(achievements);
        }

        // GET: Achievements/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var achievement = await _context.Achievement.FirstOrDefaultAsync(m => m.Id_Pasiekimas == id);
            if (achievement == null)
            {
                return NotFound();
            }

            return View(achievement);
        }

        // GET: Achievements/Create
        public IActionResult Create()
        {
            var userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            var user = _context.Users.FirstOrDefault(u => u.Id_Naudotojas == userId);

            if (user == null || user.Level < 5)
            {
                TempData["ErrorMessage"] = "You are not an administrator and cannot create achievements.";
                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        // POST: Achievements/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id_Pasiekimas,Pavadinimas,Aprasas")] Achievement achievement)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id_Naudotojas == userId);

            if (user == null || user.Level < 5)
            {
                TempData["ErrorMessage"] = "You are not an administrator and cannot create achievements.";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                achievement.UserId = (int)userId;
                achievement.Generic = user.Level == 5; // Generic is true if the user creating it has Level 5
                _context.Add(achievement);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(achievement);
        }

        // GET: Achievements/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var achievement = await _context.Achievement.FindAsync(id);
            if (achievement == null)
            {
                return NotFound();
            }

            var userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id_Naudotojas == userId);

            if (user == null || user.Level < 5)
            {
                TempData["ErrorMessage"] = "You are not an administrator and cannot edit achievements.";
                return RedirectToAction(nameof(Index));
            }

            return View(achievement);
        }

        // POST: Achievements/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id_Pasiekimas,Pavadinimas,Aprasas,UserId,Generic")] Achievement achievement)
        {
            if (id != achievement.Id_Pasiekimas)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(achievement);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AchievementExists(achievement.Id_Pasiekimas))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(achievement);
        }

        // GET: Achievements/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var achievement = await _context.Achievement.FirstOrDefaultAsync(m => m.Id_Pasiekimas == id);
            if (achievement == null)
            {
                return NotFound();
            }

            var userId = HttpContext.Session.GetInt32("UserId");
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id_Naudotojas == userId);

            if (user == null || user.Level < 5)
            {
                TempData["ErrorMessage"] = "You are not an administrator and cannot delete achievements.";
                return RedirectToAction(nameof(Index));
            }

            return View(achievement);
        }

        // POST: Achievements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var achievement = await _context.Achievement.FindAsync(id);
            if (achievement != null)
            {
                _context.Achievement.Remove(achievement);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool AchievementExists(int id)
        {
            return _context.Achievement.Any(e => e.Id_Pasiekimas == id);
        }
    }
}
