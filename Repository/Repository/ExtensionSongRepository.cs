using Microsoft.EntityFrameworkCore;
using Repository.Entity;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repository.Repository
{
    public class ExtensionSongRepository : ISongRepository
    {
        private readonly IContext _context;
        private readonly ISpeechToTextRepository _speechToTextRepository;
        private readonly ISongElasticSearchRepository _songElasticSearchRepository;

        public ExtensionSongRepository(IContext context, ISpeechToTextRepository speechToTextRepository, ISongElasticSearchRepository songElasticSearchRepository)
        {
            _context = context;
            _speechToTextRepository = speechToTextRepository;
            _songElasticSearchRepository = songElasticSearchRepository;
        }

        public async Task<Song> GetSongAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid song id.", nameof(id));

            try
            {
                return await _context.songs
                  .Include(s => s.UserLikes)
                  .FirstOrDefaultAsync(s => s.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving song.", ex);
            }
        }

        public async Task<User> GetUserAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid user id.", nameof(id));

            try
            {
                return await _context.users.FirstOrDefaultAsync(x => x.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving user.", ex);
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
                throw new Exception("Error retrieving songs.", ex);
            }
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid song id.", nameof(id));

            try
            {
                Song song = await GetSongAsync(id);
                if (song == null)
                    throw new Exception("Song not found.");

                _context.songs.Remove(song);
                await _context.SaveAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting song.", ex);
            }
        }

        public async Task<List<Song>> FilterSongAsync(string? name, bool? newSongs, bool? popularSongs, string? nameSinger, Types? type, Categories? category, TargetAge? age)
        {
            try
            {
                List<Song> listSongs = await GetAllAsync();
                if (!string.IsNullOrWhiteSpace(name))
                    listSongs = listSongs.Where(x => x.Name == name).ToList();
                if (newSongs.HasValue && newSongs.Value)
                    listSongs = listSongs.Where(x => x.Date > DateTime.Now.AddDays(-30)).ToList();

                if (popularSongs.HasValue && popularSongs.Value)
                {
                    double avgLikes = listSongs.Average(x => x.UserLikes.Count());
                    double avgViews = listSongs.Average(x => x.NumViews);
                    listSongs = listSongs.Where(x => x.UserLikes.Count() > avgLikes || x.NumViews > avgViews).ToList();
                }
                if (!string.IsNullOrWhiteSpace(nameSinger))
                {
                    var singerIds = await _context.singers
                        .Where(x => x.Name == nameSinger)
                        .Select(x => x.Id)
                        .ToListAsync();
                    listSongs = listSongs.Where(x => singerIds.Contains(x.SingerId)).ToList();
                }
                if (type != null)
                    listSongs = listSongs.Where(x => x.Type == type).ToList();
                if (category != null)
                    listSongs = listSongs.Where(x => x.Category == category).ToList();
                if (age.HasValue)
                    listSongs = listSongs.Where(x => x.TargetAge == age).ToList();
                return listSongs;
            }
            catch (Exception ex)
            {
                throw new Exception("Error filtering songs.", ex);
            }
        }

        public async Task AddLikeAsync(int idSong, int idUser)
        {
            if (idSong <= 0)
                throw new ArgumentException("Invalid song id.", nameof(idSong));
            if (idUser <= 0)
                throw new ArgumentException("Invalid user id.", nameof(idUser));

            try
            {
                Song song = await GetSongAsync(idSong);
                if (song == null)
                    throw new Exception("Song not found.");
                User user = await GetUserAsync(idUser);
                if (user == null)
                    throw new Exception("User not found.");
                if (!song.UserLikes.Any(u => u.Id == user.Id))
                {
                    song.UserLikes.Add(user);
                    user.SongLikes.Add(song);
                    await _context.SaveAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error add like.", ex);
            }
        }

        public async Task RemoveLikeAsync(int idSong, int idUser)
        {
            if (idSong <= 0)
                throw new ArgumentException("Invalid song id.", nameof(idSong));
            if (idUser <= 0)
                throw new ArgumentException("Invalid user id.", nameof(idUser));

            try
            {
                Song song = await GetSongAsync(idSong);
                if (song == null)
                    throw new Exception("Song not found.");
                User user = await GetUserAsync(idUser);
                if (user == null)
                    throw new Exception("User not found.");

                if (song.UserLikes.Any(u => u.Id == user.Id))
                {
                    song.UserLikes.Remove(user);
                    user.SongLikes.Remove(song);
                    await _context.SaveAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error removing like.", ex);
            }
        }

        public async Task AddViewAsync(int idSong)
        {
            if (idSong <= 0)
                throw new ArgumentException("Invalid song id.", nameof(idSong));

            try
            {
                Song song = await GetSongAsync(idSong);
                if (song == null)
                    throw new Exception("Song not found.");

                song.NumViews++;
                await _context.SaveAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding view.", ex);
            }
        }

        public async Task<bool> LikeByUserAsync(int idUser, int idSong)
        {
            if (idSong <= 0)
                throw new ArgumentException("Invalid song id.", nameof(idSong));
            if (idUser <= 0)
                throw new ArgumentException("Invalid user id.", nameof(idUser));

            try
            {
                Song song = await GetSongAsync(idSong);
                if (song == null)
                    throw new Exception("Song not found.");
                User user = await GetUserAsync(idUser);
                if (user == null)
                    throw new Exception("User not found.");
                if (song.UserLikes == null)
                    return false;
                return song.UserLikes.Any(u => u.Id == user.Id);
            }
            catch (Exception ex)
            {
                throw new Exception("Error checking like by user.", ex);
            }
        }

        public async Task<List<Song>> SearchSongsByAudioAsync(string audioFilePath)
        {
            if (string.IsNullOrWhiteSpace(audioFilePath))
                throw new ArgumentException("Audio file path is required.", nameof(audioFilePath));

            try
            {
                string recognizedText = await _speechToTextRepository.RecognizeSpeechAsync(audioFilePath);
                if (string.IsNullOrWhiteSpace(recognizedText))
                    return new List<Song>();

                return await _songElasticSearchRepository.SearchSongsByLyricsAsync(recognizedText);
            }
            catch (Exception ex)
            {
                throw new Exception("Error searching songs by audio.", ex);
            }
        }

        public async Task<List<Song>> GetSongsBySingerIdAsync(int singerId)
        {
            if (singerId <= 0)
                throw new ArgumentException("Invalid singer id.", nameof(singerId));

            try
            {
                return await _context.songs
                    .Where(s => s.SingerId == singerId)
                    .Include(s => s.UserLikes)
                    .Include(s => s.Feedback)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving songs by singer id.", ex);
            }
        }
    }
}
