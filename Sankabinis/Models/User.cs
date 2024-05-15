using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sankabinis.Models
{
    [Table("naudotojas")]
    public class User
    {
        [Key]
        public int Id_Naudotojas { get; set; }

        [StringLength(255)]
        public string Vardas_pavarde { get; set; }

        [StringLength(255)]
        public string Slapyvardis { get; set; }

        [StringLength(255)]
        public string Slaptazodis { get; set; }

        [StringLength(255)]
        public string El_pastas { get; set; }

        public int Elo { get; set; }

        public int Lenktyniu_skaicius { get; set; }

        public double Svoris { get; set; }

        [DataType(DataType.Date)]
        public DateTime Gimimo_data { get; set; }

        [DataType(DataType.Date)]
        public DateTime Paskyros_sukurimo_data { get; set; }

        public int Laimėta_lenktyniu { get; set; }

        public int Pralaimėta_lenktyniu { get; set; }

        public int Pasitikimo_taskai { get; set; }

        [DataType(DataType.Date)]
        public DateTime Paskutinio_prisijungimo_data { get; set; }

        public int Suspeduotos_busenos_skaicius { get; set; }

        [StringLength(7)]
        public string Lytis { get; set; }

        [StringLength(13)]
        public string Patirtis { get; set; }

        [StringLength(11)]
        public string Busena { get; set; }

        [ForeignKey("City")]
        public int CityId { get; set; }
    }
}
