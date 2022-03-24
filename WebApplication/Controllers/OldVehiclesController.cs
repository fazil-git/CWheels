using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using WebApplication.Models;
using WebApplication.Data;

namespace WebApplication.Controllers
{
    [Route("api/[controller]/")]
    [ApiController]
    public class OldVehiclesController : ControllerBase
    {
        private CWheelsDbContext _cWheelsDbContext;

        public OldVehiclesController(CWheelsDbContext cWheelsDbContext)
        {
            _cWheelsDbContext = cWheelsDbContext;
        }

        [HttpGet]
        public IActionResult GetVehicles()
        {
            return Ok(_cWheelsDbContext.Vehicles);
        }

        //Get: api/Vehicles/Test/2  
        [HttpGet("[action]/{id}")]
        public int Test(int id)
        {
            return id;
        }
        
        //Get: api/Vehicles/empty
        [Route("empty")]
        [HttpGet]
        public IActionResult emptyRequest()
        {
            return StatusCode(StatusCodes.Status401Unauthorized);
        }

        public IEnumerable<Vehicle> GetVehicles1()
        {
            return _cWheelsDbContext.Vehicles;
        }

        [HttpGet("{id}")]
        public Vehicle GetVehicle(int id)
        {
            return _cWheelsDbContext.Vehicles.Find(id);
        }

        [HttpPost]
        public IActionResult PostVehicles([FromBody] Vehicle vehicle)
        {
            _cWheelsDbContext.Vehicles.Add(vehicle);
            _cWheelsDbContext.SaveChanges();
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPut("{id}")]
        public IActionResult PutVehicles(int id, [FromBody] Vehicle vehicle)
        {
            Vehicle entity = _cWheelsDbContext.Vehicles.Find(id);
            entity.Title = vehicle.Title;
            entity.Price = vehicle.Price;
            entity.Color = vehicle.Color;
            _cWheelsDbContext.SaveChanges();
            return Ok("record updated successfully");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteVehicles(int id)
        {
            _cWheelsDbContext.Vehicles.Remove(_cWheelsDbContext.Vehicles.Find(id));
            _cWheelsDbContext.SaveChanges();
            return Ok("1 record deleted successfully");
        }

        //for study purposes
        public void Method()
        {
            List<Vehicle> vehicle = new List<Vehicle> { new Vehicle()};
         
        }
    }
}