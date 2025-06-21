using Repository.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface ISingerRepository
    {
        Task<Singer> GetByNameAndPasswordAsync(string name, string password);
        Task<bool> ValidSingerPasswordAsync(int id, string password);
    }
}
