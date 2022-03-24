using Microsoft.EntityFrameworkCore;
using WebApplication.Models;

namespace WebApplication.Data
{
    //This class is responsible for CRUD operations with database
    public class CWheelsDbContext : DbContext
    {
        public CWheelsDbContext(DbContextOptions<CWheelsDbContext> options ) : base(options)
        {

        }

        //This will create a Table with name "Vehicles" and returns a row "Vehicle"
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Image> Images { get; set; }
    }
}
