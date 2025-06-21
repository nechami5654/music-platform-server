using Repository.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface ISongElasticSearchService
    {
        Task<List<SongDto>> SearchSongsByLyricsAsync(string lyrics);
        Task<List<SongDto>> FreeSearchAsync(string text);
    }
}
