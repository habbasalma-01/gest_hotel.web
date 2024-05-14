// RoomsController.cs

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using gest_hotel.web.Models;
using gest_hotel.web.Models.Entities;
using gest_hotel.web.Data;

namespace gest_hotel.web.Controllers
{
    public class RoomsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RoomsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Rooms/RoomHotel
        public async Task<IActionResult> RoomHotel()
        {

            AddAntiCacheHeaders();
            var adminId = HttpContext.Session.GetString("AdminId");

            if (adminId != null)
            {
                Guid adminGuid = new Guid(adminId);
                var admin = _context.Admins.FirstOrDefault(a => a.Id == adminGuid);

                if (admin != null)
                {
                    var rooms = await _context.Chambres.ToListAsync();
                    return View(rooms);
                }
            }
            return RedirectToAction("Error");
        }

        // GET: Rooms/AddRoom
        [HttpGet]
        public IActionResult AddRoom()
        {
            AddAntiCacheHeaders();
            var adminId = HttpContext.Session.GetString("AdminId");

            if (adminId != null)
            {
                Guid adminGuid = new Guid(adminId);
                var admin = _context.Admins.FirstOrDefault(a => a.Id == adminGuid);

                if (admin != null)
                {
                    return View();
                }
            }
            return RedirectToAction("Error");
        }

        // POST: Rooms/AddRoom
        [HttpPost]
        public async Task<IActionResult> AddRoom(Chambre chambre, IFormFile roomPhoto)
        {

            AddAntiCacheHeaders();
            var adminId = HttpContext.Session.GetString("AdminId");

            if (adminId != null)
            {
                Guid adminGuid = new Guid(adminId);
                var admin = _context.Admins.FirstOrDefault(a => a.Id == adminGuid);

                if (admin != null)
                {

                    if (ModelState.IsValid)
                    {
                        _context.Chambres.Add(chambre);
                        await _context.SaveChangesAsync();

                        if (roomPhoto != null && roomPhoto.Length > 0)
                        {
                            var fileName = chambre.Id + Path.GetExtension(roomPhoto.FileName);
                            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/room", fileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await roomPhoto.CopyToAsync(stream);
                            }
                        }

                        return RedirectToAction("RoomHotel", "Admin");
                    }
                    return View(chambre);
                }
            }
            return RedirectToAction("Error");
        }

        // GET: Rooms/Delete/5
        public async Task<IActionResult> DeleteRoom(int? id)
        {

            AddAntiCacheHeaders();
            var adminId = HttpContext.Session.GetString("AdminId");

            if (adminId != null)
            {
                Guid adminGuid = new Guid(adminId);
                var admin = _context.Admins.FirstOrDefault(a => a.Id == adminGuid);

                if (admin != null)
                {
                    if (id == null)
                    {
                        return NotFound();
                    }

                    var chambre = await _context.Chambres
                        .FirstOrDefaultAsync(m => m.Id == id);
                    if (chambre == null)
                    {
                        return NotFound();
                    }

                    return View(chambre);
                }
            }
            return RedirectToAction("Error");
        }
        // POST: Rooms/Delete/5
        [HttpPost, ActionName("DeleteRoom")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRoomConfirmed(int id)
        {

            AddAntiCacheHeaders();
            var adminId = HttpContext.Session.GetString("AdminId");

            if (adminId != null)
            {
                Guid adminGuid = new Guid(adminId);
                var admin = _context.Admins.FirstOrDefault(a => a.Id == adminGuid);

                if (admin != null)
                {
                    var chambre = await _context.Chambres.FindAsync(id);
                    if (chambre == null)
                    {
                        return NotFound();
                    }

                    // Supprimer l'image correspondant à la chambre du dossier wwwroot/Images/room
                    var fileName = id + ".jpeg";
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/room", fileName);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }

                    _context.Chambres.Remove(chambre);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("RoomHotel", "Admin"); // Mettre à jour le chemin vers RoomHotel.cshtml
                }
            }
            return RedirectToAction("Error");
        }
        public async Task<IActionResult> EditRoom(int? id)
        {
            AddAntiCacheHeaders();
            var adminId = HttpContext.Session.GetString("AdminId");

            if (adminId != null)
            {
                Guid adminGuid = new Guid(adminId);
                var admin = _context.Admins.FirstOrDefault(a => a.Id == adminGuid);

                if (admin != null)
                {
                    if (id == null)
                    {
                        return NotFound();
                    }

                    var chambre = await _context.Chambres.FindAsync(id);
                    if (chambre == null)
                    {
                        return NotFound();
                    }

                    return View(chambre);
                }
            }
            return RedirectToAction("Error");
        }
        private bool ChambreExists(int id)
        {
            return _context.Chambres.Any(e => e.Id == id);
        }// POST: Rooms/EditRoom/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRoom(int id, [Bind("Id,TypeChambre,Description,PrixParNuit,Disponibilite")] Chambre chambre)
        {
            AddAntiCacheHeaders();
            var adminId = HttpContext.Session.GetString("AdminId");

            if (adminId != null)
            {
                Guid adminGuid = new Guid(adminId);
                var admin = _context.Admins.FirstOrDefault(a => a.Id == adminGuid);

                if (admin != null)
                {
                    if (id != chambre.Id)
                    {
                        return NotFound();
                    }

                    if (ModelState.IsValid)
                    {
                        try
                        {
                            // Mettre à jour les informations de la chambre
                            _context.Update(chambre);
                            await _context.SaveChangesAsync();
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            if (!ChambreExists(chambre.Id))
                            {
                                return NotFound();
                            }
                            else
                            {
                                throw;
                            }
                        }
                        return RedirectToAction("RoomHotel", "Admin"); // Mettre à jour le chemin vers RoomHotel.cshtml
                    }
                    return View(chambre);
                }
            }
            return RedirectToAction("Error");
        }

        private void AddAntiCacheHeaders()
        {
            Response.Headers["Cache-Control"] = "no-cache, no-store";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "-1";
        }
    }
}