namespace gest_hotel.web.Models.Entities
{
    public class Client
    {
        public Guid Id { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public string password { get; set; }
        public bool ReservationConfirmee { get; set; } // Ajout de la propriété ReservationConfirmee
    }
}