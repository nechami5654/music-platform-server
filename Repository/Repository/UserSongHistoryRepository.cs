using Microsoft.EntityFrameworkCore;
using Repository.Entity;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repository.Repository
{
    public class UserSongHistoryRepository : IRepository<UserSongHistory>
    {
        private readonly IContext _context;
        public UserSongHistoryRepository(IContext context)
        {
            _context = context;
        }

        public async Task<UserSongHistory> AddAsync(UserSongHistory item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            try
            {
                var existingHistory = await _context.userSongHistories.FirstOrDefaultAsync(h => h.UserId == item.UserId && h.SongId == item.SongId);

                if (existingHistory != null)
                {
                    existingHistory.PlayedAt = DateTime.Now;
                }
                else
                {
                    item.PlayedAt = DateTime.Now;
                    await _context.userSongHistories.AddAsync(item);
                }

                await _context.SaveAsync();
                return existingHistory ?? item;
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding user song history.", ex);
            }
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid id.", nameof(id));

            try
            {
                var history = await GetAsync(id);
                if (history == null)
                    throw new Exception("User song history not found.");

                _context.userSongHistories.Remove(history);
                await _context.SaveAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting user song history.", ex);
            }
        }

        public async Task<UserSongHistory> GetAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid id.", nameof(id));

            try
            {
                return await _context.userSongHistories
                    .Where(x => x.UserId == id)
                    .OrderByDescending(x => x.PlayedAt) 
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving user song history.", ex);
            }
        }

        public async Task<List<UserSongHistory>> GetAllAsync()
        {
            try
            {
                return await _context.userSongHistories
                    .OrderByDescending(h => h.PlayedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving all user song histories.", ex);
            }
        }

        public async Task<UserSongHistory> UpdateAsync(int id, UserSongHistory item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (id <= 0)
                throw new ArgumentException("Invalid id.", nameof(id));

            try
            {
                var history = await GetAsync(id);
                if (history == null)
                    throw new Exception("User song history not found.");
                history.PlayedAt = DateTime.Now;
                await _context.SaveAsync();
                return history;
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating user song history.", ex);
            }
        }
    }
}
