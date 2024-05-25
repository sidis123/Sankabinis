using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sankabinis.Data;
using Sankabinis.Models;
using Microsoft.Extensions.Configuration;

namespace Sankabinis.Controllers
{
    public class CityController : Controller
    {
        private readonly SankabinisContext _context;
        public CityController(SankabinisContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CheckIfExists(string cityPavadinimas)
        {
            var existingCity = _context.City.FirstOrDefault(c => c.Pavadinimas == cityPavadinimas);
            if (existingCity == null)
            {
                var newCity = new City
                {
                    Pavadinimas = cityPavadinimas,
                    Koordinates = ""
                };
                _context.City.Add(newCity);
                _context.SaveChanges();

                var googleController = new GoogleApiController(_context, null);
                googleController.FindDistance(newCity);
            }

            return Ok();
        }

    }
}