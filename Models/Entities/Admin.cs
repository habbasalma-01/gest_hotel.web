namespace gest_hotel.web.Models.Entities

{
    public class Admin
    {
        public Guid Id { get; set; }
        public string firstname { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }

        public string lastname { get; set; }
    }
}