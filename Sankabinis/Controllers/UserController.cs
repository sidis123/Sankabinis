using Microsoft.AspNetCore.Mvc;
using Sankabinis.Controllers;
using Sankabinis.Data;
using Sankabinis.Models;

public class UserController : Controller
{
    private readonly SankabinisContext _context;

    public UserController(SankabinisContext context)
    {
        _context = context;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]

    public IActionResult RegistrationPage()
    {
        return View();
    }
    public IActionResult ValidateRegistration(User user)
    {
        if (string.IsNullOrEmpty(user.Slapyvardis))
        {
            return View("RegistrationPage", user);
        }
        if (string.IsNullOrEmpty(user.Slaptazodis))
        {
            return View("RegistrationPage", user);
        }

        if (_context.Users.Any(u => u.Slapyvardis == user.Slapyvardis))
        {
            ModelState.AddModelError("Slapyvardis", "This slapyvardis is already taken.");
            return View("RegistrationPage", user);
        }

        CreateAccount(user);
        return NavigateToSignInPage(user);
    }
    private IActionResult CreateAccount(User user)
    {
        user.Paskyros_sukurimo_data = DateTime.Now;
        user.Busena = "Aktyvi";
        user.Elo = 0;
        user.Lenktyniu_skaicius = 0;
        user.Laimėta_lenktyniu = 0;
        user.Pralaimėta_lenktyniu = 0;
        user.Pasitikimo_taskai = 200;
        user.Paskutinio_prisijungimo_data = DateTime.Now;
        user.Suspeduotos_busenos_skaicius = 0;
        user.El_pastas = "";
        user.Vardas_pavarde = "";
        user.Gimimo_data = DateTime.Now;
        user.Lytis = "";
        user.Patirtis = "";
        user.Svoris = 0;

        _context.Users.Add(user);
        _context.SaveChanges();

        return Ok();
    }
    public IActionResult NavigateToSignInPage(User user)
    {
        return View("SignInPage", user);
    }

    public IActionResult ValidateSignIn(User user)
    {
        if (string.IsNullOrEmpty(user.Slapyvardis))
        {
            return View("SignInPage", user);
        }
        if (string.IsNullOrEmpty(user.Slaptazodis))
        {
            return View("SignInPage", user);
        }
        var existingUser = _context.Users.FirstOrDefault(u => u.Slapyvardis == user.Slapyvardis && u.Slaptazodis == user.Slaptazodis);
        if (existingUser == null)
        {
            ModelState.AddModelError("Slaptazodis", "Invalid username or password.");
            return View("SignInPage", user);
        }

        SignIn(user, existingUser);
        return View("~/Views/Home/Index.cshtml");
    }

    public IActionResult SignIn(User user, User existingUser)
    {
        HttpContext.Session.SetString("Username", user.Slapyvardis);
        HttpContext.Session.SetInt32("UserId", existingUser.Id_Naudotojas);
        return Ok();    
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }

    public IActionResult ValidateData(User user, string cityPavadinimas)
    {
        if (string.IsNullOrEmpty(user.Vardas_pavarde) ||
            string.IsNullOrEmpty(user.El_pastas) ||
            string.IsNullOrEmpty(user.Patirtis) ||
            string.IsNullOrEmpty(cityPavadinimas))
        {
            ModelState.AddModelError(string.Empty, "Please fill in all required fields.");
            return View("ProfileCreationPage", user);
        }
        var loggedInUserId = HttpContext.Session.GetInt32("UserId");
        var loggedInUser = _context.Users.FirstOrDefault(u => u.Id_Naudotojas == loggedInUserId);

        var cityController = new CityController(_context);
        cityController.CheckIfExists(cityPavadinimas);

        loggedInUser.Vardas_pavarde = user.Vardas_pavarde;
        loggedInUser.El_pastas = user.El_pastas;
        loggedInUser.Patirtis = user.Patirtis;
        loggedInUser.Svoris = user.Svoris;
        loggedInUser.Gimimo_data = user.Gimimo_data;

        var city = _context.City.FirstOrDefault(c => c.Pavadinimas == cityPavadinimas);
        if (city != null)
        {
            loggedInUser.CityId = city.Id_Miestas;
        }
        _context.SaveChanges();

        return RedirectToAction("Index", "Home");
    }



}
