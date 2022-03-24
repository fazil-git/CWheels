using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication.Models
{
    public class Image
    {
        [Key]
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public int VehicleId { get; set; }
        //this will be excluded in the table data base schema
        [NotMapped]
        public byte[] ImageArray { get; set; }
    }
}
