using gest_hotel.web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace gest_hotel.web.Controllers
{
    public class ClientController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClientController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Action pour afficher les chambres disponibles aux clients
        public IActionResult ClientHome()
        {
            AddAntiCacheHeaders();
            var clientId = HttpContext.Session.GetString("ClientId");
            if (clientId != null)
            {
                Guid clientGuid = new Guid(clientId);
                var client = _context.Clients.FirstOrDefault(c => c.Id == clientGuid);
                if (client != null)
                {
                    ViewBag.Nom = client.Nom;
                    ViewBag.Prenom = client.Prenom;

                    var availableRooms = _context.Chambres.Where(c => c.Disponibilite).ToList();
                    return View(availableRooms);
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

        // Autres actions du contrôleur...
    }
}