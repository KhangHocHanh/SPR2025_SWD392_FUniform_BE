using ClothingCustomization.Data;
using Microsoft.EntityFrameworkCore;

namespace ClothingCustomization.Repository
{
    public class RoleRepository : IRoleRepository
    {
        private readonly ClothesCusShopContext _context;

        public RoleRepository(ClothesCusShopContext context)
        {
            _context = context;
        }

        public async Task<Role?> GetRoleById(int id)
        {
            return await _context.Roles
                .FirstOrDefaultAsync(u => u.RoleId == id);
        }


        public async Task<List<Role>> GetRoles(string? search = null, string? sortBy = null, bool descending = false, int page = 1, int pageSize = 10)
        {
            var query = _context.Roles.AsQueryable();

            // 🔹 Filtering (Search by Role Name)
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(r => r.RoleName.Contains(search));
            }

            // 🔹 Sorting (Customizable)
            if (!string.IsNullOrEmpty(sortBy))
            {
                query = descending
                    ? query.OrderByDescending(r => EF.Property<object>(r, sortBy))
                    : query.OrderBy(r => EF.Property<object>(r, sortBy));
            }

            // 🔹 Pagination (Skip & Take)
            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            return await query.ToListAsync();
        }



    }
}
