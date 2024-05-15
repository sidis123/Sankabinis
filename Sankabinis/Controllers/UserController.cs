using Microsoft.AspNetCore.Mvc;
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

        SignIn(user);
        return View("~/Views/Home/Index.cshtml");
    }

    public IActionResult SignIn(User user)
    {
        HttpContext.Session.SetString("Username", user.Slapyvardis);
        return Ok();    
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }




}
