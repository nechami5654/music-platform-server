using Microsoft.EntityFrameworkCore;
using Repository.Entity;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repository.Repository
{
    public class SongRepository : IRepository<Song>
    {
        private readonly IContext _context;
        private readonly ISongElasticSearchRepository _elasticRepository;

        public SongRepository(IContext context, ISongElasticSearchRepository elasticRepository)
        {
            _context = context;
            _elasticRepository = elasticRepository;
        }

        private async Task AddIndexSongWrapperAsync(Song item)
        {
            await _elasticRepository.AddIndexSongAsync(item);
        }

        private async Task DeleteIndexSongWrapperAsync(int id)
        {
            await _elasticRepository.DeleteIndexSongAsync(id);
        }

        private async Task UpdateIndexSongWrapperAsync(Song item)
        {
            await _elasticRepository.UpdateIndexSongAsync(item);
        }

        public async Task<Song> AddAsync(Song item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            try
            {
                item.Date = DateTime.Now;
                await _context.songs.AddAsync(item);
                await _context.SaveAsync();
                _ = AddIndexSongWrapperAsync(item);
                return item;
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding song.", ex);
            }
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid song id.", nameof(id));

            try
            {
                var song = await GetAsync(id);
                if (song == null)
                    throw new Exception("Song not found.");

                _context.songs.Remove(song);
                await _context.SaveAsync();
                _ = DeleteIndexSongWrapperAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting song.", ex);
            }
        }

        public async Task<Song> GetAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid song id.", nameof(id));

            try
            {
                return await _context.songs
                   .Include(s => s.UserLikes) 
                   .Include(s => s.Feedback) 
                   .FirstOrDefaultAsync(s => s.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving song.", ex);
            }
        }

        public async Task<List<Song>> GetAllAsync()
        {
            try
            {
                return await _context.songs.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving all songs.", ex);
            }
        }

        public async Task<Song> UpdateAsync(int id, Song item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (id <= 0)
                throw new ArgumentException("Invalid song id.", nameof(id));

            try
            {
                Song s = await GetAsync(id);
                if (s == null)
                    throw new Exception("Song not found.");

                s.Name = item.Name;
                s.Words = item.Words;
                s.VideoUrl = item.VideoUrl;
                s.Type = item.Type;
                s.Category = item.Category;
                s.TargetAge = item.TargetAge;
                await _context.SaveAsync();
                _ = UpdateIndexSongWrapperAsync(s);
                return s;
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating song.", ex);
            }
        }
    }
}
