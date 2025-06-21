using Microsoft.EntityFrameworkCore;
using Repository.Entity;
using Repository.Interface;
using System;
using System.Threading.Tasks;

namespace Repository.Repository
{
    public class ExtensionSingerRepository : ISingerRepository
    {
        private readonly IContext _context;
        public ExtensionSingerRepository(IContext context)
        {
            _context = context;
        }

        public async Task<Singer> GetByNameAndPasswordAsync(string name, string password)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is required.", nameof(name));
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password is required.", nameof(password));

            try
            {
                return await _context.singers.FirstOrDefaultAsync(x => x.Name == name && x.Password == password);
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving singer by name and password.", ex);
            }
        }
        public async Task<bool> ValidSingerPasswordAsync(int id, string password)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid singer ID.", nameof(id));
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password is required.", nameof(password));

            try
            {
                return await _context.singers.AnyAsync(x => x.Id == id && x.Password == password);
            }
            catch (Exception ex)
            {
                throw new Exception("Error validating singer credentials.", ex);
            }
        }
    }
}
