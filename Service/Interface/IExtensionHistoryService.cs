using Repository.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IExtensionHistoryService
    {
        Task<bool> GetAsync(int idUser, int idSong);
        Task<List<SongDto>> GetAllAsync(int idUser);
    }
}
