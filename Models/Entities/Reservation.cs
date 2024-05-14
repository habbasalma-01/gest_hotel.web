using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace gest_hotel.web.Models.Entities
{
    public class Reservation
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("Client")]
        public Guid ClientId { get; set; }
        public virtual Client Client { get; set; }

        [ForeignKey("Chambre")]
        public int ChambreId { get; set; }
        public virtual Chambre Chambre { get; set; }

        [Required(ErrorMessage = "La date d'arrivée est obligatoire")]
        public DateTime DateArrivee { get; set; }

        [Required(ErrorMessage = "La date de départ est obligatoire")]
        public DateTime DateDepart { get; set; }

        [Required(ErrorMessage = "Le statut de la réservation est obligatoire")]
        public string Statut { get; set; } // Confirmée, Annulée, En attente, etc.

        [Required(ErrorMessage = "Le montant total est obligatoire")]
        public decimal MontantTotal { get; set; }

        // Ajoutez d'autres propriétés si nécessaire, comme des détails de paiement, etc.
    }
}
