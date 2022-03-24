using System.Collections.Generic;

namespace WebApplication.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string ImageUrl { get; set; }
        public ICollection<Vehicle> Vehicles { get; set; }
    }
}
