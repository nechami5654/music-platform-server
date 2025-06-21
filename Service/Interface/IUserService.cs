using Repository.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IUserService
    {
        string Generate(UserDto user);
        Task<UserDto> AuthenticateAsync(string userName, string pass);
        Task<List<UserDto>> GetUsersByIdsAsync(List<int> ids);
        Task<bool> ValidUserPasswordAsync(int id, string password);
    }
}
