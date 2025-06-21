using Repository.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface ISingerService
    {
        string Generate(SingerDto singer);
        Task<SingerDto> AuthenticateAsync(string singerName, string pass);
        Task<bool> ValidSingerPasswordAsync(int id, string password);

    }
}
