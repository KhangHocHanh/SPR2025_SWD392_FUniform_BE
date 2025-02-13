using ClothingCustomization.Data;

namespace ClothingCustomization.Repository
{
    public interface IUserRepository
    {
        Task<User> Login(string taikhoan, string matkhau);
        Task<User> Register(User user);
        Task<User?> GetUserById(int id);
        Task<List<User>> GetUsers();

    }
}
