using ImageUploader;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private CWheelsDbContext _cWheelsDbContext;
        public ImagesController(CWheelsDbContext cWheelsDbContext)
        {
            _cWheelsDbContext = cWheelsDbContext;
        }

        [HttpPost]
        [Authorize]
        public IActionResult Post([FromBody] Image imageModel)
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
            var user = _cWheelsDbContext.Users.FirstOrDefault(u => u.Email == userEmail);
            if (user == null) return NotFound();
            FileStream fs = new FileStream("C:\\Program", FileMode.Append, FileAccess.ReadWrite); //for study reference
            var stream = new MemoryStream(imageModel.ImageArray);
            var guid = Guid.NewGuid().ToString();
            var file = $"{guid}.jpg";
            var folder = "Images";
            var response = FilesHelper.UploadImage(stream, folder, file);
            if (!response)
            {
                return BadRequest();
            }
            else
            {
                var image = new Image()
                {
                    Id = imageModel.Id,
                    ImageUrl = file,
                    VehicleId = imageModel.VehicleId
                };
                _cWheelsDbContext.Images.Add(image);
                _cWheelsDbContext.SaveChanges();
                return StatusCode(StatusCodes.Status201Created, "Image added successfully!");
            }
        }
    }
}
