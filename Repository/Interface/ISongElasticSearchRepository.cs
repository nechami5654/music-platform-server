using Repository.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface ISongElasticSearchRepository
    {
        Task<List<Song>> SearchSongsByLyricsAsync(string lyrics);
        Task AddIndexSongAsync(Song song);
        Task DeleteIndexSongAsync(int id);
        Task UpdateIndexSongAsync(Song song);
        Task<List<Song>> FreeSearchAsync(string text);
    }
}
