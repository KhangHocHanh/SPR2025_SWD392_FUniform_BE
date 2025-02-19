using ClothingCustomization.Data;
using ClothingCustomization.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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
            return await _context.Users
                .Include(x => x.Role)
                .FirstOrDefaultAsync(u => u.Username == taikhoan && u.Password == matkhau);
        }

        public async Task<User> Register(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> GetUserById(int id)
        {
            return await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == id);
        }

        public async Task<List<User>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User?> UpdateUser(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

    }
}
