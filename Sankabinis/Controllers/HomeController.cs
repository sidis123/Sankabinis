using GoogleApi.Entities.Search.Common;
using Microsoft.AspNetCore.Mvc;
using Sankabinis.Models;
using System.Diagnostics;

namespace Sankabinis.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (TempData.ContainsKey("Message"))
            {
                ViewBag.Message = TempData["Message"];
            }
            return View();
        }
        public IActionResult NavigateToRegistrationPage()
        {
            return View("~/Views/User/RegistrationPage.cshtml");
        }
        public IActionResult NavigateToSignInPage()
        {
            return View("~/Views/User/SignInPage.cshtml");
        }
        public IActionResult NavigateToUserRacesPage(int userId)
        {
            //int loggedinId = userId;
            return RedirectToAction("MatchListPage", "Race", new { loggedinId = userId });
            //return View("~/Views/Race/MatchListPage.cshtml");
        }
        public IActionResult NavigateToProfileCreationPage()
        {
            return View("~/Views/User/ProfileCreationPage.cshtml");
        }
        public IActionResult NavigateToProfilePage()
        {
            return View("~/Views/User/ProfilePage.cshtml");
        }
        public IActionResult NavigateToLeaderPage()
        {
            return View("~/Views/Leaders/LeaderPage.cshtml");
        }
        public IActionResult NavigateToAchievementPage()
        {
            return View("~/Views/Achievements/AchievementPage.cshtml");
        }

        public IActionResult NavigateToRacePage()
        {
            return RedirectToAction("Index", "Race");
        }

        public IActionResult NavigateToAppealListPage()
        {
            return RedirectToAction("Index", "Appeal");
        }

        //For testng
        public IActionResult NavigateToTimeChoicePage()
        {
            Race race = new Race();
            return RedirectToAction("Index", "TimeChoice", race);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
