using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sankabinis.Models
{
    [Table("trasa")]
    public class Track
    {
        [Key]
        public int Id_Trasa{ get; set; }

        [ForeignKey("City")]
        public int CityId { get; set; }

        public string start_coordinates { get; set; }

        public string finish_coordinates { get; set; }

        public string name { get; set; }

        public string image { get; set; }
    }
}
