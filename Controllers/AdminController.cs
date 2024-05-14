using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using gest_hotel.web.Models.Entities;
using gest_hotel.web.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Filters;
using gest_hotel.web.Migrations;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using gest_hotel.web.Models;

namespace gest_hotel.web.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public AdminController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // Action pour afficher le tableau de bord de l'admin
        public IActionResult Admin(string firstName, string lastName)
        {
            AddAntiCacheHeaders();
            var adminId = HttpContext.Session.GetString("AdminId");

            if (adminId != null)
            {
                Guid adminGuid = new Guid(adminId);
                var adminn = _context.Admins.FirstOrDefault(a => a.Id == adminGuid);

                if (adminn != null)
                {
                    ViewBag.FirstName = firstName;
                    ViewBag.LastName = lastName;
                    return View("~/Views/Admin/Dashboard.cshtml");
                }
            }
            return RedirectToAction("Error");
        }

        public IActionResult AdminDashboard()
        {
            AddAntiCacheHeaders();
            var adminId = HttpContext.Session.GetString("AdminId");

            if (adminId != null)
            {
                Guid adminGuid = new Guid(adminId);
                var adminn = _context.Admins.FirstOrDefault(a => a.Id == adminGuid);

                if (adminn != null)
                {
                    return View();
                }
            }
            return RedirectToAction("Error");
        }

        // Action pour afficher le formulaire pour ajouter un nouvel admin
        public IActionResult AddAdmin()
        {
            AddAntiCacheHeaders();
            var adminId = HttpContext.Session.GetString("AdminId");

            if (adminId != null)
            {
                Guid adminGuid = new Guid(adminId);
                var adminn = _context.Admins.FirstOrDefault(a => a.Id == adminGuid);

                if (adminn != null)
                {
                    return View();
                }
            }
            return RedirectToAction("Error");
        }

        // Action pour traiter la soumission du formulaire pour ajouter un nouvel admin
        [HttpPost]
        public IActionResult AddAdmin(Admin admin)
        {
            AddAntiCacheHeaders();
            var adminId = HttpContext.Session.GetString("AdminId");

            if (adminId != null)
            {
                Guid adminGuid = new Guid(adminId);
                var adminn = _context.Admins.FirstOrDefault(a => a.Id == adminGuid);

                if (adminn != null)
                {
                    if (ModelState.IsValid)
                    {
                        _context.Admins.Add(admin);
                        _context.SaveChanges(); // Sauvegarde dans la base de données
                        return RedirectToAction("Admin");
                    }
                    AddAntiCacheHeaders();
                    return View(admin);
                }
            }
            return RedirectToAction("Error");
        }

        // Action pour afficher les clients
        public IActionResult ViewClients()
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

        public IActionResult RoomHotel()
        {
            AddAntiCacheHeaders();
            var adminId = HttpContext.Session.GetString("AdminId");

            if (adminId != null)
            {
                Guid adminGuid = new Guid(adminId);
                var admin = _context.Admins.FirstOrDefault(a => a.Id == adminGuid);

                if (admin != null)
                {


                    var chambres = _context.Chambres.ToList();

                    return View(chambres);
                }
            }
            return RedirectToAction("Error");
        }

        [HttpGet]
        public IActionResult ClientInformation()
        {
            AddAntiCacheHeaders();
            var adminId = HttpContext.Session.GetString("AdminId");

            if (adminId != null)
            {
                Guid adminGuid = new Guid(adminId);
                var admin = _context.Admins.FirstOrDefault(a => a.Id == adminGuid);

                if (admin != null)
                {

                    var clients = _context.Clients.ToList();

                    return View(clients);
                }
            }
            return RedirectToAction("Error");
        }

        // Action pour afficher le profil de l'administrateur
        [HttpGet]
        public IActionResult ViewProfile()
        {
            AddAntiCacheHeaders();
            var adminId = HttpContext.Session.GetString("AdminId");

            if (adminId != null)
            {
                Guid adminGuid = new Guid(adminId);
                var admin = _context.Admins.FirstOrDefault(a => a.Id == adminGuid);

                if (admin != null)
                {
                    return View(admin);
                }
            }

            return RedirectToAction("Error");
        }
        public IActionResult Dashboard()
        {
            AddAntiCacheHeaders();
            var adminId = HttpContext.Session.GetString("AdminId");

            if (adminId != null)
            {
                Guid adminGuid = new Guid(adminId);
                var adminn = _context.Admins.FirstOrDefault(a => a.Id == adminGuid);

                if (adminn != null)
                {
                    var totalReservations = _context.Reservations.Count();
                    var totalClients = _context.Clients.Count();
                    var totalEarnings = _context.Reservations.Sum(r => r.MontantTotal);
                    var totalAdmins = _context.Admins.Count(); // Assurez-vous que vous avez une table 'Admins'

                    // Assurez-vous que le total des clients n'est pas supérieur au total des administrateurs
                    if (totalClients > totalAdmins)
                    {
                        totalClients = totalAdmins;
                    }

                    var model = new DashboardViewModel
                    {
                        TotalClients = totalClients,
                        TotalReservations = totalReservations,
                        TotalEarnings = totalEarnings,
                        TotalAdmins = totalAdmins
                    };

                    return View(model);
                }
            }
            return RedirectToAction("Error");
        }


        public IActionResult Reservations()
        {

            AddAntiCacheHeaders();
            var adminId = HttpContext.Session.GetString("AdminId");

            if (adminId != null)
            {
                Guid adminGuid = new Guid(adminId);
                var adminn = _context.Admins.FirstOrDefault(a => a.Id == adminGuid);

                if (adminn != null)
                {
                    var reservations = _context.Reservations.Include(r => r.Client).ToList();
                    return View(reservations);
                }
            }
            return RedirectToAction("Error");
        }
        [HttpPost]
        public IActionResult CancelReservation(Guid id)
        {
            AddAntiCacheHeaders();
            var adminId = HttpContext.Session.GetString("AdminId");

            if (adminId != null)
            {
                Guid adminGuid = new Guid(adminId);
                var adminn = _context.Admins.FirstOrDefault(a => a.Id == adminGuid);

                if (adminn != null)
                {
                    var reservation = _context.Reservations.Find(id);
                    if (reservation != null)
                    {
                        _context.Reservations.Remove(reservation);
                        _context.SaveChanges();
                    }
                    return RedirectToAction("Reservations");
                }
            }
            return RedirectToAction("Error");
        }

        // Méthode pour ajouter les en-têtes anti-cache
        private void AddAntiCacheHeaders()
        {
            Response.Headers["Cache-Control"] = "no-cache, no-store";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "-1";
        }
    }
}