using Repository.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface ISongService
    {
        Task<List<SongDto>> FilterSongAsync(string? name, bool? newSongs, bool? popularSongs, string? nameSinger, Types? type, Categories? category, TargetAge? age);
        Task AddViewAsync(int idSong);
        Task AddLikeAsync(int idSong, int idUser);
        Task RemoveLikeAsync(int idSong, int idUser);
        Task<bool> LikeByUserAsync(int idUser, int idSong);
        Task<List<Song>> SearchSongsByAudioAsync(string audioFilePath);
        Task<List<SongDto>> GetSongsBySingerIdAsync(int singerId);
    }
}
