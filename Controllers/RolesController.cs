using ClothingCustomization.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClothingCustomization.Controllers
{
    public class RolesController : Controller
    {
        private readonly ClothesCusShopContext _context;
        public RolesController(ClothesCusShopContext context)
        {
            _context = context;
        }

        /*
        [HttpGet]
        [Route("Roles")]
        public IActionResult GetRoles()
        {
            
        }

        [HttpGet]
        [Route("Roles/{id}")]
        public IActionResult GetRoles(int id)
        {
            
        }
        */
    }
}
