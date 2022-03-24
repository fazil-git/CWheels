using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using WebApplication.Data;

namespace WebApplication.Controllers
{
    
    [ApiController]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private CWheelsDbContext _cWheelsDbContext;
        public CategoriesController(CWheelsDbContext cWheelsDbContext)
        {
            _cWheelsDbContext = cWheelsDbContext;
        }

        [Route("api/getCategories")]
        [HttpGet]
        public IActionResult GetCategories()
        {
            return Ok(_cWheelsDbContext.Categories);
        }

        [Route("api/getCategory/{id}")]
        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetCategorys(int id)
        {
            return Ok(_cWheelsDbContext.Categories.Where(c => c.Id == id));
        }
    }
}
