using gest_hotel.web.Data;
using gest_hotel.web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MimeKit;

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

using gest_hotel.web.Migrations;

using System.Net.Mail;
using System.Net;
using gest_hotel.web.Models.Entities;
using Org.BouncyCastle.Crypto.Engines;
using System;

using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using iText.Commons.Actions.Contexts;

namespace gest_hotel.web.Controllers
{
    public class ReservationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public ReservationController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult MyReservations()
        {
            AddAntiCacheHeaders();
            var clientId = HttpContext.Session.GetString("ClientId");
            if (clientId != null && Guid.TryParse(clientId, out Guid clientGuid))
            {
                var clientReservations = _context.Reservations
                    .Include(r => r.Chambre)
                    .Where(r => r.ClientId == clientGuid)
                    .ToList();

                return View(clientReservations);
            }
            return RedirectToAction("Error");
        }

        [HttpGet]
        public IActionResult Reserve(int chambreId)
        {
            AddAntiCacheHeaders();
            var clientId = HttpContext.Session.GetString("ClientId");
            if (clientId != null && Guid.TryParse(clientId, out Guid clientGuid))
            {
                ViewBag.ChambreId = chambreId;
                HttpContext.Session.SetInt32("CodeConfirmation", new Random().Next(100000, 999999));
                return View(chambreId);
            }
            return RedirectToAction("Error");
        }

        [HttpPost]
        public async Task<IActionResult> Reserve(int chambreId, DateTime dateArrivee, DateTime dateDepart)
        {
            AddAntiCacheHeaders();
            var clientId = HttpContext.Session.GetString("ClientId");
            if (clientId != null && Guid.TryParse(clientId, out Guid clientGuid))
            {
                var chambre = await _context.Chambres.FindAsync(chambreId);

                if (chambre != null)
                {
                    var reservations = await _context.Reservations
                        .Where(r => r.ChambreId == chambreId &&
                                    ((dateArrivee >= r.DateArrivee && dateArrivee < r.DateDepart) ||
                                     (dateDepart > r.DateArrivee && dateDepart <= r.DateDepart) ||
                                     (dateArrivee <= r.DateArrivee && dateDepart >= r.DateDepart)))
                        .ToListAsync();

                    if (reservations.Count == 0)
                    {
                        var nombreDeJours = (dateDepart - dateArrivee).TotalDays;
                        var montantTotal = (decimal)nombreDeJours * chambre.PrixParNuit;

                        ViewBag.Disponibilite = true;
                        ViewBag.MontantTotal = montantTotal;
                    }
                    else
                    {
                        ViewBag.Disponibilite = false;
                    }
                }
                else
                {
                    ViewBag.Disponibilite = false;
                }

                ViewBag.ChambreId = chambreId;
                return Json(new { Disponibilite = ViewBag.Disponibilite, MontantTotal = ViewBag.MontantTotal });
            }
            return RedirectToAction("Error");
        }

        [HttpGet]
        public IActionResult CheckAvailability(int chambreId, DateTime arrivalDate, DateTime departureDate)
        {
            AddAntiCacheHeaders();
            var clientId = HttpContext.Session.GetString("ClientId");
            if (clientId != null && Guid.TryParse(clientId, out Guid clientGuid))
            {
                var chambre = _context.Chambres.Find(chambreId);
                if (chambre != null)
                {
                    var reservations = _context.Reservations
                        .Where(r => r.ChambreId == chambreId &&
                                    ((arrivalDate >= r.DateArrivee && arrivalDate < r.DateDepart) ||
                                     (departureDate > r.DateArrivee && departureDate <= r.DateDepart) ||
                                     (arrivalDate <= r.DateArrivee && departureDate >= r.DateDepart)))
                        .ToList();

                    if (reservations.Count == 0)
                    {
                        var nombreDeJours = (departureDate - arrivalDate).TotalDays;
                        var montantTotal = (decimal)nombreDeJours * chambre.PrixParNuit;
                        return Json(new { Disponibilite = true, MontantTotal = montantTotal });
                    }
                }

                return Json(new { Disponibilite = false });
            }
            return RedirectToAction("Error");
        }

        [HttpPost]
        public IActionResult ConfirmReservation(int chambreId, string dateArrivee, string dateDepart, decimal montantTotal)
        {
            try
            {
                HttpContext.Session.SetInt32("ChambreId", chambreId);
                HttpContext.Session.SetString("DateArrivee", dateArrivee.ToString());
                HttpContext.Session.SetString("DateDepart", dateDepart.ToString());
                HttpContext.Session.SetString("MontantTotal", montantTotal.ToString());

                var clientId = HttpContext.Session.GetString("ClientId");
                if (clientId != null)
                {
                    Guid clientGuid = new Guid(clientId);
                    var client = _context.Clients.FirstOrDefault(c => c.Id == clientGuid);

                    if (client != null)
                    {
                        Random random = new Random();
                        int code = random.Next(100000, 999999);
                        HttpContext.Session.SetInt32("ReservationCode", code);

                        using (MailMessage message = new MailMessage())
                        {
                            message.From = new MailAddress("radissonh119@gmail.com");
                            message.To.Add(new MailAddress(client.Email));
                            message.Subject = "Code de confirmation de réservation";
                            message.Body = "Votre code de confirmation de réservation est : " + code;

                            using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587))
                            {
                                smtpClient.UseDefaultCredentials = false;
                                smtpClient.Credentials = new NetworkCredential("radissonh119@gmail.com", "ybqgheeekgripukg");
                                smtpClient.EnableSsl = true;

                                smtpClient.Send(message);
                            }
                        }

                        return View();
                    }
                }

                return RedirectToAction("Error");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while confirming the reservation: " + ex.Message);
                return RedirectToAction("Error");
            }
        }

        [HttpPost]
        public IActionResult bank(int chambreId, DateTime dateArrivee, DateTime dateDepart, decimal montantTotal)
        {
            ViewBag.ChambreId = chambreId;
            ViewBag.DateArrivee = dateArrivee;
            ViewBag.DateDepart = dateDepart;
            ViewBag.MontantTotal = montantTotal;

            return View();
        }


        [HttpPost]
        public IActionResult SubmitVerificationCode(int verificationCode)
        {
            try
            {
                if (verificationCode == HttpContext.Session.GetInt32("ReservationCode"))
                {
                    var chambreId = HttpContext.Session.GetInt32("ChambreId").GetValueOrDefault();
                    var dateArrivee = Convert.ToDateTime(HttpContext.Session.GetString("DateArrivee"));
                    var dateDepart = Convert.ToDateTime(HttpContext.Session.GetString("DateDepart"));

                    var chambre = _context.Chambres.Find(chambreId);

                    if (chambre != null)
                    {
                        var nombreDeJours = (dateDepart - dateArrivee).TotalDays;
                        var montantTotal = (decimal)nombreDeJours * chambre.PrixParNuit;

                        var clientId = HttpContext.Session.GetString("ClientId");
                        var clientGuid = new Guid(clientId);

                        var reservation = new Reservation
                        {
                            Id = Guid.NewGuid(),
                            ClientId = clientGuid,
                            ChambreId = chambreId,
                            DateArrivee = dateArrivee,
                            DateDepart = dateDepart,
                            MontantTotal = montantTotal,
                            Statut = "confirmer"
                        };

                        _context.Reservations.Add(reservation);
                        _context.SaveChanges();

                        // Générer le PDF de la réservation
                        var pdfFilePath = GenerateReservationPDF(chambre, dateArrivee, dateDepart,montantTotal);

                        if (!string.IsNullOrEmpty(pdfFilePath))
                        {
                            // Envoyer l'e-mail avec le PDF en pièce jointe
                            SendReservationConfirmationEmail(clientGuid, pdfFilePath);

                            // Rediriger vers la page d'accueil du client
                            return RedirectToAction("ClientHome", "Client");
                        }
                        else
                        {
                            // Gérer l'erreur de génération du PDF
                            ModelState.AddModelError("", "Une erreur s'est produite lors de la génération du PDF de la réservation.");
                            return RedirectToAction("Error");
                        }
                    }
                    else
                    {
                        // Gérer l'absence de la chambre dans la base de données
                        ModelState.AddModelError("", "La chambre sélectionnée n'est pas disponible.");
                        return RedirectToAction("ConfirmReservation");
                    }
                }
                else
                {
                    // Le code de vérification est incorrect
                    ModelState.AddModelError("verificationCode", "Le code de vérification est incorrect.");
                    return RedirectToAction("ConfirmReservation");
                }
            }
            catch (Exception ex)
            {
                // Gérer l'exception
                Console.WriteLine("Une erreur s'est produite lors de la confirmation de la réservation : " + ex.Message);
                return RedirectToAction("Error");
            }
        }



        private string GenerateReservationPDF(Chambre chambre, DateTime dateArrivee, DateTime dateDepart, decimal montantTotal)
        {
            try
            {
                // Spécifier le chemin complet du fichier PDF
                string fileName = "Reservation.pdf";
                string directoryPath = Environment.CurrentDirectory;
                string filePath = Path.Combine(directoryPath, fileName);

                // Créer le document PDF
                using (var stream = new FileStream(filePath, FileMode.Create))
                using (var doc = new Document())
                {
                    PdfWriter.GetInstance(doc, stream);
                    doc.Open();

                    // Créer une table pour centrer l'image en haut du PDF
                    PdfPTable table = new PdfPTable(1);
                    table.WidthPercentage = 100;
                    PdfPCell cell = new PdfPCell();
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;

                    // Spécifier le chemin complet du logo
                    string logoPath = @"C:\Users\salma\source\repos\gest_hotel.web\wwwroot\Images\imageHoma.jpeg";

                    try
                    {
                        Image logo = Image.GetInstance(logoPath);
                        logo.ScaleToFit(100f, 100f); // Redimensionner l'image si nécessaire
                        cell.AddElement(logo);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Une erreur s'est produite lors de l'ajout du logo : {ex.Message}");
                    }

                    table.AddCell(cell);
                    doc.Add(table);

                    // Ajouter les informations de la chambre dans le PDF
                    doc.Add(new Paragraph("                                 -----------------------------------------------------------------------------"));
                    doc.Add(new Paragraph("                                     Votre réservation chez l'Hôtel Radisson a été effectuée avec succès"));
                    doc.Add(new Paragraph("                                                     Type de chambre : " + chambre.TypeChambre));
                    doc.Add(new Paragraph("                                                     Description : " + chambre.Description));
                    doc.Add(new Paragraph("                                                     Montant total : " + montantTotal + " DH"));
                    doc.Add(new Paragraph("                                                     Date d'arrivée : " + dateArrivee.ToShortDateString()));
                    doc.Add(new Paragraph("                                                     Date de départ : " + dateDepart.ToShortDateString()));
                    doc.Add(new Paragraph("                                 -----------------------------------------------------------------------------"));

                    doc.Close();
                }

                return filePath; // Retourner le chemin du fichier PDF généré
            }
            catch (Exception ex)
            {
                Console.WriteLine("Une erreur s'est produite lors de la génération du PDF : " + ex.Message);
                return null; // Retourner null en cas d'erreur
            }
        }




        // Méthode pour envoyer l'e-mail de confirmation de réservation avec le PDF en pièce jointe
        private void SendReservationConfirmationEmail(Guid clientGuid, string pdfFilePath)
        {
            try
            {
                var client = _context.Clients.FirstOrDefault(c => c.Id == clientGuid);

                if (client != null)
                {
                    // Créer et configurer le message e-mail
                    using (MailMessage message = new MailMessage())
                    {
                        message.From = new MailAddress("radissonh119@gmail.com");
                        message.To.Add(new MailAddress(client.Email));
                        message.Subject = "Confirmation de réservation";
                        message.Body = "Votre réservation est confirmée. Veuillez trouver ci-joint le détail de votre réservation.";

                        // Ajouter le PDF en pièce jointe
                        Attachment attachment = new Attachment(pdfFilePath);
                        message.Attachments.Add(attachment);

                        // Envoyer l'e-mail
                        using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587))
                        {
                            smtpClient.UseDefaultCredentials = false;
                            smtpClient.Credentials = new NetworkCredential("radissonh119@gmail.com", "ybqgheeekgripukg");
                            smtpClient.EnableSsl = true;
                            smtpClient.Send(message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Gérer l'erreur d'envoi de l'e-mail
                Console.WriteLine("Une erreur s'est produite lors de l'envoi de l'e-mail de confirmation de réservation : " + ex.Message);
            }
        }


        private void AddAntiCacheHeaders()
        {
            Response.Headers["Cache-Control"] = "no-cache, no-store";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "-1";
        }
    }
}
