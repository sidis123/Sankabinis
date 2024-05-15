using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sankabinis.Models
{
    [Table("miestas")]
    public class City
    {
        [Key]
        public int Id_Miestas { get; set; }

        [Required(ErrorMessage = "Pavadinimas is required")]
        [StringLength(255)]
        public string Pavadinimas { get; set; }

        [StringLength(255)]
        public string Koordinates { get; set; }
    }
}