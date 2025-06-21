using Microsoft.EntityFrameworkCore;
using Repository.Entity;
using Repository.Interface;
using System;
using System.Threading.Tasks;

namespace Repository.Repository
{
    public class ExtensionUserRepository : IUserRepository
    {
        private readonly IContext _context;
        public ExtensionUserRepository(IContext context)
        {
            _context = context;
        }

        public async Task<User> GetByNameAndPasswordAsync(string name, string password)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is required.", nameof(name));
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password is required.", nameof(password));

            try
            {
                return await _context.users.FirstOrDefaultAsync(x => x.Name == name && x.Password == password);
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving user by name and password.", ex);
            }
        }

        public async Task<List<User>> GetUsersByIdsAsync(List<int> ids)
        {
            if (ids == null || ids.Count == 0)
                throw new ArgumentException("Invalid user IDs list.", nameof(ids));

            try
            {
                return await _context.users.Where(x => ids.Contains(x.Id)).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving users by IDs.", ex);
            }
        }

        public async Task<bool> ValidUserPasswordAsync(int id, string password)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid user ID.", nameof(id));
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password is required.", nameof(password));

            try
            {
                return await _context.users.AnyAsync(x => x.Id == id && x.Password == password);
            }
            catch (Exception ex)
            {
                throw new Exception("Error validating user credentials.", ex);
            }
        }
    }
}
