using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sankabinis.Models
{
    [Table("automobilis")]
    public class Car
    {
        [Key]
        public int Id_Automobilis { get; set; }

        [Required, StringLength(50)]
        public string Modelis { get; set; }

        [Required, StringLength(50)]
        public string Marke { get; set; }

        [Required, StringLength(9)]
        public string Numeris { get; set; }

        [Range(1, 1200)]
        public int Galingumas { get; set; }

        [Required, StringLength(50)]
        public string Spalva { get; set; }

        [Range(0, int.MaxValue)]
        public int Rida { get; set; }

        [DataType(DataType.Date)]
        public DateTime Pagaminimo_data { get; set; }

        [Range(1, 50000)]
        public double Svoris { get; set; }

        public KuroTipas Kuro_tipas { get; set; }

        public PavaruDeze Pavaru_deze { get; set; }

        public Kebulas Kebulas { get; set; }

        // This property is calculated based on `Svoris` and `Galingumas`.
        public AutomobilioKlase Klase { get; set; }

        [Required]
        public int Fk_Naudotojasid_Naudotojas { get; set; }
    }

    public enum KuroTipas
    {
        Benzinas = 1,
        BenzinasElektra = 2,
        Dyzelis = 3,
        Dujos = 4,
        Elektra = 5
    }

    public enum PavaruDeze
    {
        Automatine = 1,
        Mechanine = 2
    }

    public enum Kebulas
    {
        Sedanas = 1,
        Hecbeakas = 2,
        Universalas = 3,
        Visureigis = 4,
        Pikapas = 5,
        Kupe = 6,
        Kabrioletas = 7,
        Vienaturis = 8
    }

    public enum AutomobilioKlase
    {
        Lupena = 1,
        Kasdiene = 2,
        Idomesne = 3,
        Superine = 4
    }
}
