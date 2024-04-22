using System.ComponentModel.DataAnnotations;

namespace Sankabinis.Models
{
    public class Automobilis
    {
        [Key]
        public int Id_Automobilis { get; set; }

        [StringLength(255)]
        public string Modelis { get; set; }

        [StringLength(255)]
        public string Marke { get; set; }

        [StringLength(255)]
        public string Numeris { get; set; }

        public int Galingumas { get; set; }

        [StringLength(255)]
        public string Spalva { get; set; }

        public int Rida { get; set; }

        [DataType(DataType.Date)]
        public DateTime Pagaminimo_data { get; set; }

        public double Svoris { get; set; }

        public KuroTipas Kuro_tipas { get; set; }

        public PavaruDeze Pavaru_deze { get; set; }

        public Kebulas Kebulas { get; set; }

        public AutomobilioKlase Klase { get; set; }

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
