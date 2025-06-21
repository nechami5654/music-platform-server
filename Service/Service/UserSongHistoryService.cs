using AutoMapper;
using Repository.Entity;
using Repository.Interface;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Service
{
    public class UserSongHistoryService : IService<UserSongHistoryDto>
    {
        private readonly IRepository<UserSongHistory> _repository;
        private readonly IMapper _mapper;

        public UserSongHistoryService(IRepository<UserSongHistory> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<UserSongHistoryDto> AddAsync(UserSongHistoryDto item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            try
            {
                var historyEntity = _mapper.Map<UserSongHistory>(item);
                var addedHistory = await _repository.AddAsync(historyEntity);
                return _mapper.Map<UserSongHistoryDto>(addedHistory);
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding user song history.", ex);
            }
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid id.", nameof(id));

            try
            {
                await _repository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting user song history.", ex);
            }
        }

        public async Task<UserSongHistoryDto> GetAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid id.", nameof(id));

            try
            {
                var history = await _repository.GetAsync(id);
                if (history == null)
                    throw new Exception("User song history not found.");

                return _mapper.Map<UserSongHistoryDto>(history);
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving user song history.", ex);
            }
        }

        public async Task<List<UserSongHistoryDto>> GetAllAsync()
        {
            try
            {
                var histories = await _repository.GetAllAsync();
                return _mapper.Map<List<UserSongHistoryDto>>(histories);
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving user song histories.", ex);
            }
        }

        public async Task<UserSongHistoryDto> UpdateAsync(int id, UserSongHistoryDto item)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid id.", nameof(id));
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            try
            {
                var historyEntity = _mapper.Map<UserSongHistory>(item);
                var updatedHistory = await _repository.UpdateAsync(id, historyEntity);
                return _mapper.Map<UserSongHistoryDto>(updatedHistory);
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating user song history.", ex);
            }
        }
    }
}
