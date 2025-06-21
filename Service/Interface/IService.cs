using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IService<T>
    {
        Task<List<T>> GetAllAsync();
        Task<T> GetAsync(int id);
        Task<T> UpdateAsync(int id, T item);
        Task DeleteAsync(int id);
        Task<T> AddAsync(T item);
    }
}
