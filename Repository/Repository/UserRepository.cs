using Microsoft.EntityFrameworkCore;
using Repository.Entity;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repository.Repository
{
    public class UserRepository : IRepository<User>
    {
        private readonly IContext _context;
        public UserRepository(IContext context)
        {
            _context = context;
        }

        public async Task<User> AddAsync(User item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            try
            {
                await _context.users.AddAsync(item);
                await _context.SaveAsync();
                return item;
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding user.", ex);
            }
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid id.", nameof(id));

            try
            {
                var user = await GetAsync(id);
                if (user == null)
                    throw new Exception("User not found.");

                _context.users.Remove(user);
                await _context.SaveAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting user.", ex);
            }
        }

        public async Task<User> GetAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid id.", nameof(id));

            try
            {
                return await _context.users.FirstOrDefaultAsync(x => x.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving user.", ex);
            }
        }

        public async Task<List<User>> GetAllAsync()
        {
            try
            {
                return await _context.users.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving users.", ex);
            }
        }

        public async Task<User> UpdateAsync(int id, User item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (id <= 0)
                throw new ArgumentException("Invalid id.", nameof(id));

            try
            {
                var user = await GetAsync(id);
                if (user == null)
                    throw new Exception("User not found.");

                user.Name = item.Name;
                user.Password = item.Password;
                user.Email = item.Email;
                user.ImageUrl = item.ImageUrl;
                await _context.SaveAsync();
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating user.", ex);
            }
        }
    }
}
