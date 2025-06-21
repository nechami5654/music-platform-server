using Microsoft.EntityFrameworkCore;
using Repository.Entity;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repository.Repository
{
    public class SingerRepository : IRepository<Singer>
    {
        private readonly IContext _context;
        public SingerRepository(IContext context)
        {
            _context = context;
        }

        public async Task<Singer> AddAsync(Singer item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            try
            {
                await _context.singers.AddAsync(item);
                await _context.SaveAsync();
                return item;
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding singer in repository.", ex);
            }
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid id", nameof(id));

            try
            {
                var singer = await GetAsync(id);
                if (singer == null)
                    throw new Exception("Singer not found.");

                _context.singers.Remove(singer);
                await _context.SaveAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting singer in repository.", ex);
            }
        }

        public async Task<Singer> GetAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid id", nameof(id));

            try
            {
                return await _context.singers.FirstOrDefaultAsync(x => x.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving singer in repository.", ex);
            }
        }

        public async Task<List<Singer>> GetAllAsync()
        {
            try
            {
                return await _context.singers.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving singers in repository.", ex);
            }
        }

        public async Task<Singer> UpdateAsync(int id, Singer item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (id <= 0)
                throw new ArgumentException("Invalid id", nameof(id));

            try
            {
                Singer s = await GetAsync(id);
                if (s == null)
                    throw new Exception("Singer not found.");

                s.Name = item.Name;
                s.Password = item.Password;
                s.Email = item.Email;
                s.ImageUrl = item.ImageUrl;
                await _context.SaveAsync();
                return s;
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating singer in repository.", ex);
            }
        }
    }
}
