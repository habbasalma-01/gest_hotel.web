using Microsoft.AspNetCore.Mvc;
using System.Linq;
using gest_hotel.web.Models.Entities;
using gest_hotel.web.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using gest_hotel.web.Migrations;

namespace gest_hotel.web.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var admin = _context.Admins.FirstOrDefault(a => a.Email == email && a.Password == password);
            var client = _context.Clients.FirstOrDefault(c => c.Email == email && c.password == password);

            if (client != null)
            {
                // Connecter le client et rediriger vers la page d'accueil par exemple
                HttpContext.Session.SetString("ClientId", client.Id.ToString());
                return RedirectToAction("ClientHome", "Client");
            }
            else if (admin != null)
            {
                // Stocker l'ID de l'administrateur dans la session
                HttpContext.Session.SetString("AdminId", admin.Id.ToString());
                return RedirectToAction("Dashboard", "Admin");
            }
            else
            {
                // Identifiants invalides, afficher un message d'erreur
                ModelState.AddModelError(string.Empty, "Email ou mot de passe incorrect.");
                return View();
            }
        }


        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Register(Client client)
        {
            if (ModelState.IsValid)
            {
                _context.Clients.Add(client);
                _context.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return View(client);
        }


        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            // Clear the authentication cookies
            HttpContext.Session.Remove("AdminId");
            HttpContext.Session.Remove("ClientId");

            // Redirect to the login page
            return RedirectToAction("Login", "Account");

        }
        public IActionResult gallery()
        {
            return View();
        }


    }
}