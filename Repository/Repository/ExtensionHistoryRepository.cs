using Elastic.Clients.Elasticsearch;
using Microsoft.EntityFrameworkCore;
using Repository.Entity;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repository
{
    public class ExtensionHistoryRepository : IExtensionHistoryRepository
    {
        private readonly IContext _context;
        public ExtensionHistoryRepository(IContext context)
        {
            _context = context;
        }
        public async Task<bool> GetAsync(int idUser, int idSong)
        {
            if (idUser <= 0 || idSong <= 0)
                throw new ArgumentException("Invalid id.", nameof(idUser));

            try
            {
                return await _context.userSongHistories.FirstOrDefaultAsync(x => x.UserId == idUser && x.SongId == idSong) != null;
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving user song history.", ex);
            }
        }

        public async Task<List<Song>> GetAllAsync(int idUser)
        {
            if (idUser <= 0)
                throw new ArgumentException("Invalid id.", nameof(idUser));

            try
            {
                var userSongHistory = await _context.userSongHistories
                    .Where(x => x.UserId == idUser)
                    .Select(x => x.SongId) 
                    .ToListAsync();

                if (!userSongHistory.Any()) 
                {
                    return new List<Song>(); 
                }

                var songs = await _context.songs
                    .Where(x => userSongHistory.Contains(x.Id)) 
                    .ToListAsync();

                return songs;
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving user song history.", ex);
            }
        }

    }
}
