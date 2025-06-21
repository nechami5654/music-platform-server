using Repository.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface IExtensionHistoryRepository
    {
        Task<bool> GetAsync(int idUser, int idSong);
        Task<List<Song>> GetAllAsync(int idUser);
    }
}
