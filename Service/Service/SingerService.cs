using AutoMapper;
using Repository.Entity;
using Repository.Interface;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Service
{
    public class SingerService : IService<SingerDto>
    {
        private readonly IRepository<Singer> _repository;
        private readonly IMapper _mapper;
        public SingerService(IRepository<Singer> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<SingerDto> AddAsync(SingerDto item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            try
            {
                var singerEntity = _mapper.Map<Singer>(item);
                var result = await _repository.AddAsync(singerEntity);
                return _mapper.Map<SingerDto>(result);
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding singer.", ex);
            }
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid id", nameof(id));

            try
            {
                await _repository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting singer.", ex);
            }
        }

        public async Task<SingerDto> GetAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid id", nameof(id));

            try
            {
                var singer = await _repository.GetAsync(id);
                if (singer == null)
                    throw new Exception("Singer not found.");

                return _mapper.Map<SingerDto>(singer);
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting singer.", ex);
            }
        }

        public async Task<List<SingerDto>> GetAllAsync()
        {
            try
            {
                var singers = await _repository.GetAllAsync();
                return _mapper.Map<List<SingerDto>>(singers);
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting singers.", ex);
            }
        }

        public async Task<SingerDto> UpdateAsync(int id, SingerDto item)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid id", nameof(id));
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            try
            {
                var singerEntity = _mapper.Map<Singer>(item);
                var updatedSinger = await _repository.UpdateAsync(id, singerEntity);
                return _mapper.Map<SingerDto>(updatedSinger);
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating singer.", ex);
            }
        }
    }
}
