using ClothingCustomization.Data;
using Microsoft.EntityFrameworkCore;

namespace ClothingCustomization.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ClothesCusShopContext _context;

        public UserRepository(ClothesCusShopContext context)
        {
            _context = context;
        }

        public async Task<User> Login(string taikhoan, string matkhau)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == taikhoan && u.Password == matkhau);
        }

        public async Task<User> Register(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> GetUserById(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);
        }

        public async Task<List<User>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }
    }
}
