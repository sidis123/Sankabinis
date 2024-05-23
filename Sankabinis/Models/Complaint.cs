using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sankabinis.Models
{
    [Table("skundas")]
    public class Complaint
    {
        [Key]
        public int Id_Skundas { get; set; }

        [StringLength(255)]
        public string Paaiskinimas { get; set; }


        [DataType(DataType.Date)]
        public DateTime Sukurimo_Data { get; set; }

        [DefaultValue(false)]
        public bool Uzdarytas { get; set; }

        [ForeignKey("Id_Lenktynes")]
        public int Id_Lenktynes{ get; set; }

    }
}
