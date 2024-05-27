using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sankabinis.Data;
using Sankabinis.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckIfExists(string cityPavadinimas)
        {
            var cityExists = _context.City.Any(c => c.Pavadinimas == cityPavadinimas);
            if (!cityExists)
            {
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "lt.json");
                var json = System.IO.File.ReadAllText(filePath);
                var citiesJson = JsonConvert.DeserializeObject<List<CityJson>>(json);

                var selectedCityObject = citiesJson.FirstOrDefault(city => city.city == cityPavadinimas);

                City city = new City
                {
                    Pavadinimas = selectedCityObject.city,
                    Koordinates = selectedCityObject.lat + ", " + selectedCityObject.lng
                };


                _context.City.Add(city);

                _context.SaveChanges();


                _googleApiController.FindDistance(city);

            }

            return Ok();
        }
    }
}