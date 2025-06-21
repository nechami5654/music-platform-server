using Repository.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface IUserRepository
    {
        Task<User> GetByNameAndPasswordAsync(string name, string password);
        Task<List<User>> GetUsersByIdsAsync(List<int> ids);
        Task<bool> ValidUserPasswordAsync(int id, string password);
    }
}
