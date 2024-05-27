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
        private readonly GoogleApiController _googleApiController;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public CityController(GoogleApiController googleApiController, SankabinisContext context, IWebHostEnvironment webHostEnvironment)
        {
            _googleApiController = googleApiController;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }

        public  async Task<IActionResult> CheckIfExists(string cityPavadinimas)
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

                await _googleApiController.FindDistance(newCity);
            }

            return Ok();
        }

    }
}