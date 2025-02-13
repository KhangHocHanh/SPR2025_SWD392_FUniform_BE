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

        [HttpGet]
        [Route("GetUser")]
        public IActionResult GetUser(int id)
        {
            var user = _context.Users.FirstOrDefault(x => x.UserId == id);
            if (user != null)
            {
                return Ok(user);
            }
            else
            {
                return NoContent();
            }
        }
    }
}
