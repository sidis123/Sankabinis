using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sankabinis.Models
{
    [Table("lenktynes")]
    public class Race
    {
        [Key]
        public int Id_Lenktynes { get; set; }

        [ForeignKey("User")]
        public int User1Id { get; set; }

        [ForeignKey("User")]
        public int User2Id { get; set; }

        [DefaultValue(false)]
        public bool Pirmo_naudotojo_patvirtinimas { get; set; }
        [DefaultValue(false)]
        public bool Antro_naudotojo_patvirtinimas { get; set; }

        public AutomobilioKlase Automobilio_klase { get; set; }

        public DateTime pasiulytas_laikas { get; set; }

        [DefaultValue(false)]
        public bool ar_laikas_patvirtintas { get; set; }

        [DefaultValue(false)]
        public bool ar_lenktynes_pasibaigusios { get; set; }
        [DefaultValue(100)]
        public int rezultatas_pagal_pirmaji_naudotoja { get; set; }
        [DefaultValue(100)]
        public int rezultatas_pagal_antraji_naudotoja { get; set; }

        [DefaultValue(false)]
        public bool ar_galutinis_rezultatas { get; set; }

        [ForeignKey("Track")]
        public int TrackId { get; set; }
    }
}
