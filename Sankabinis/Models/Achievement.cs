using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sankabinis.Models
{
    [Table("pasiekimas")]
    public class Achievement
    {
        [Key]
        public int Id_Pasiekimas { get; set; }

        [Required(ErrorMessage = "Pavadinimas is required")]
        [StringLength(255)]
        public string Pavadinimas { get; set; }

        [StringLength(255)]
        public string Aprasas { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        [DefaultValue(false)]
        public bool Generic { get; set; }
    }
}