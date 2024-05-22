using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sankabinis.Models
{
    [Table("atstumas")]
    public class Distance
    {
        [Key]
        public int Id_Atstumas{ get; set; }
        
        [ForeignKey("City")]
        public int CityId1 { get; set; }

        [ForeignKey("City")]
        public int CityId2 { get; set; }

        public double Atstumas { get; set; }
    }
}
