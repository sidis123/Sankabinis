using GoogleApi.Entities.Search.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Sankabinis.Models;
using System.Diagnostics;

namespace Sankabinis.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
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
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "lt.json");
            var json = System.IO.File.ReadAllText(filePath);
            var citiesJson = JsonConvert.DeserializeObject<List<CityJson>>(json);

            var cityNames = citiesJson.Select(city => city.city).ToList();
            ViewBag.CityNames = new SelectList(cityNames);
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
