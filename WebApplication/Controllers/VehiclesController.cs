using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class VehiclesController : ControllerBase
    {
        private CWheelsDbContext _cWheelsDbContext;

        public VehiclesController(CWheelsDbContext cWheelsDbContext)
        {
            _cWheelsDbContext = cWheelsDbContext;
        }

        [HttpPost]
        public IActionResult Post(Vehicle vehicle)
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
            var user = _cWheelsDbContext.Users.FirstOrDefault(u => u.Email == userEmail);
            if (user == null) return NotFound();
            var vehicleObj = new Vehicle()
            {
                Title = vehicle.Title,
                Price = vehicle.Price,
                Color = vehicle.Color,
                Description = vehicle.Description,
                Model = vehicle.Model,
                Engine = vehicle.Engine,
                Company = vehicle.Company,
                DatePosted = vehicle.DatePosted,
                IsHotAndNew = vehicle.IsHotAndNew,
                IsFeatured = vehicle.IsFeatured,
                Location = vehicle.Location,
                Condition = vehicle.Condition,
                UserId = user.Id,
                CategoryId = vehicle.CategoryId,
                Images = vehicle.Images,
            };
            _cWheelsDbContext.Vehicles.Add(vehicleObj);
            _cWheelsDbContext.SaveChanges();
            return Ok(new
            {
                vehicleId = vehicleObj.Id,
                message = "Vehicle added successfully"
            });

        }

        [HttpGet]
        [Authorize]
        public IActionResult HotAndNewAds()
        {
            //this picks few returns those
            var vehicles = from v in _cWheelsDbContext.Vehicles
                           where v.IsHotAndNew == true
                           select new
                           {
                               VehicleId = v.Id,
                               VehicleTitle = v.Title,
                               VehicleImage = v.Images.FirstOrDefault().ImageUrl
                           };

            //this picks all & return few
            var veh = _cWheelsDbContext.Vehicles.Where(v => v.IsHotAndNew == true);
            return Ok(veh.Select(veh => new
            {
                VehicleId = veh.Id,
                VehicleTitle = veh.Title,
                VehicleImage = veh.Images.FirstOrDefault().ImageUrl
            })
            );
        }

        [HttpGet("{searchString}")]
        [Authorize]
        public IActionResult SearchVehicles(string searchString) //when searching vehicle in the search bar
        {
            var vehicles = from v in _cWheelsDbContext.Vehicles
                           where v.Title == searchString
                           select new
                           {
                               VehicleId = v.Id,
                               VehicleTitle = v.Title
                           };
            return Ok(vehicles);
        }

        [HttpGet("{id}"), Authorize]
        public IActionResult GetUserVehicles(int id)
        {
            //this checks whether the holder of that particular bearer token is authorized
            //each user will have an unique bearer token
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
            var user = _cWheelsDbContext.Users.FirstOrDefault(u => u.Email == userEmail);
            if (user == null) return NotFound();

            //But here any user can any user's vehicles details if the pass the id of the user
            var vehicles = from u in _cWheelsDbContext.Users
                           where u.Id == id
                           join v in _cWheelsDbContext.Vehicles
                           on u.Id equals v.UserId
                           select new
                           {
                               User = user.Id,
                               UserId = u.Id,
                               Title = v.Title,
                               Price = v.Price,
                               Color = v.Color,
                               Description = v.Description,
                               Model = v.Model,
                               Engine = v.Engine,
                               Company = v.Company,
                               DatePosted = v.DatePosted,
                               IsHotAndNew = v.IsHotAndNew,
                               IsFeatured = v.IsFeatured,
                               Location = v.Location,
                               Condition = v.Condition,
                               CategoryId = v.CategoryId,
                               Images = v.Images.FirstOrDefault().ImageUrl
                           };
            return Ok(vehicles);
        }
    }
}