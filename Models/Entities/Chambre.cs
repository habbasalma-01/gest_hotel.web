using System.ComponentModel.DataAnnotations;

namespace gest_hotel.web.Models.Entities;
public class Chambre
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Le type de chambre est obligatoire")]
    public string TypeChambre { get; set; }

    [Required(ErrorMessage = "La description est obligatoire")]
    public string Description { get; set; }

    [Required(ErrorMessage = "Le prix par nuit est obligatoire")]
    public decimal PrixParNuit { get; set; }

    [Required(ErrorMessage = "La disponibilité est obligatoire")]
    public bool Disponibilite { get; set; }

  
}