using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Sankabinis.Data;
using Sankabinis.Models;

namespace Sankabinis.Controllers
{
    public class TracksController : Controller
    {
        private readonly GoogleApiController _googleApiController;
        private readonly SankabinisContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public TracksController(GoogleApiController googleApiController, SankabinisContext context, IWebHostEnvironment webHostEnvironment)
        {
            _googleApiController = googleApiController;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var tracks = _context.Track.ToList();
            return View(tracks);
        }

        public IActionResult CreateTrack()
        {
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "lt.json");
            var json = System.IO.File.ReadAllText(filePath);
            var citiesJson = JsonConvert.DeserializeObject<List<CityJson>>(json);

            var cityNames = citiesJson.Select(city => city.city).ToList();
            ViewBag.CityNames = new SelectList(cityNames);

            return View("Create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTrack(Track track, string cityName)
        {
            if (ModelState.IsValid)
            {
                
                var cityExists = _context.City.Any(c => c.Pavadinimas == cityName);
                if (!cityExists)
                {
                    var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "lt.json");
                    var json = System.IO.File.ReadAllText(filePath);
                    var citiesJson = JsonConvert.DeserializeObject<List<CityJson>>(json);

                    var selectedCityObject = citiesJson.FirstOrDefault(city => city.city == cityName);

                    City city = new City
                    {
                        Pavadinimas = selectedCityObject.city,
                        Koordinates = selectedCityObject.lat + ", " + selectedCityObject.lng
                    };


                    _context.City.Add(city);

                    _context.SaveChanges();

                    track.CityId = city.Id_Miestas;

                    _context.Track.Add(track);
                    _context.SaveChanges();


                    await _googleApiController.FindDistance(city);

                }
                track.CityId = _context.City.FirstOrDefault(c => c.Pavadinimas == cityName).Id_Miestas;
                _context.Track.Add(track);
                _context.SaveChangesAsync();
                return Json(new { success = true, track = track });
            }
            else
            {
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "lt.json");
                var json = System.IO.File.ReadAllText(filePath);
                var citiesJson = JsonConvert.DeserializeObject<List<CityJson>>(json);

                var cityNames = citiesJson.Select(city => city.city).ToList();
                ViewBag.CityNames = new SelectList(cityNames);

                return View("Create");
            }
        }
        public async Task<IActionResult> EditTrack(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var track = await _context.Track.FindAsync(id);
            if (track == null)
            {
                return NotFound();
            }
            return PartialView("_EditTrackPartial", track);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var track = await _context.Track // Include the Races navigation property
                .FirstOrDefaultAsync(m => m.Id_Trasa == id);

            if (track == null)
            {
                return NotFound();
            }

            var races = _context.Race.Where(r => r.TrackId == track.Id_Trasa).ToList();

            if (races.Count == 0)
            {
                _context.Track.Remove(track);
                await _context.SaveChangesAsync();
                return Ok("Track deleted successfully.");
            }
            else
            {
                return BadRequest("Cannot delete track. Races are associated with this track.");
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitEditedData(Track track)
        {
                _context.Update(track);
                await _context.SaveChangesAsync();
                TempData["alerts"] = "Track edited successfully.";
                var tracks = _context.Track.ToList();
                return RedirectToAction(nameof(Index));
        }

        private bool TrackExists(int id)
        {
            return _context.Track.Any(e => e.Id_Trasa == id);
        }
    }

}
