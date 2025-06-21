using AutoMapper;
using Repository.Entity;
using Repository.Interface;
using Service.Dto;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Service
{
    public class SongService : IService<SongDto>
    {
        private readonly IRepository<Song> _repository;
        private readonly IMapper _mapper;
        public SongService(IRepository<Song> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<SongDto> AddAsync(SongDto item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            try
            {
                var songEntity = _mapper.Map<Song>(item);
                var addedSong = await _repository.AddAsync(songEntity);
                return _mapper.Map<SongDto>(addedSong);
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding song.", ex);
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
                throw new Exception("Error deleting song.", ex);
            }
        }

        public async Task<SongDto> GetAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid id.", nameof(id));

            try
            {
                var song = await _repository.GetAsync(id);
                if (song == null)
                    throw new Exception("Song not found.");
                return _mapper.Map<SongDto>(song);
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving song.", ex);
            }
        }

        public async Task<List<SongDto>> GetAllAsync()
        {
            try
            {
                var songs = await _repository.GetAllAsync();
                return _mapper.Map<List<SongDto>>(songs);
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving songs.", ex);
            }
        }

        public async Task<SongDto> UpdateAsync(int id, SongDto item)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid id.", nameof(id));
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            try
            {
                var songEntity = _mapper.Map<Song>(item);
                var updatedSong = await _repository.UpdateAsync(id, songEntity);
                return _mapper.Map<SongDto>(updatedSong);
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating song.", ex);
            }
        }
    }
}
