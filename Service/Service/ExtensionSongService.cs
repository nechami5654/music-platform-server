using AutoMapper;
using Repository.Entity;
using Repository.Interface;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Service
{
    public class ExtensionSongService : ISongService
    {
        private readonly IMapper _mapper;
        private readonly ISongRepository _songRepository;

        public ExtensionSongService(IMapper mapper, ISongRepository songRepository)
        {
            _mapper = mapper;
            _songRepository = songRepository;
        }

        public async Task<List<SongDto>> FilterSongAsync(string? name, bool? newSongs, bool? popularSongs, string? nameSinger, Types? type, Categories? category, TargetAge? age)
        {
            try
            {
                var songs = await _songRepository.FilterSongAsync(name, newSongs, popularSongs, nameSinger, type, category, age);
                return _mapper.Map<List<SongDto>>(songs);
            }
            catch (Exception ex)
            {
                throw new Exception("Error filtering songs.", ex);
            }
        }

        public async Task<bool> LikeByUserAsync(int idUser, int idSong)
        {
            try
            {
                return await _songRepository.LikeByUserAsync(idUser, idSong);
            }
            catch (Exception ex)
            {
                throw new Exception("Error checking if user liked the song.", ex);
            }
        }

        public async Task RemoveLikeAsync(int idSong, int idUser)
        {
            try
            {
                await _songRepository.RemoveLikeAsync(idSong, idUser);
            }
            catch (Exception ex)
            {
                throw new Exception("Error removing like from song.", ex);
            }
        }

        public async Task AddLikeAsync(int idSong, int idUser)
        {
            try
            {
                await _songRepository.AddLikeAsync(idSong, idUser);
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding like to song.", ex);
            }
        }

        public async Task AddViewAsync(int idSong)
        {
            try
            {
                await _songRepository.AddViewAsync(idSong);
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding view to song.", ex);
            }
        }

        public async Task<List<Song>> SearchSongsByAudioAsync(string audioFilePath)
        {
            if (string.IsNullOrWhiteSpace(audioFilePath))
                throw new ArgumentException("Audio file path is required.", nameof(audioFilePath));

            try
            {
                return await _songRepository.SearchSongsByAudioAsync(audioFilePath);
            }
            catch (Exception ex)
            {
                throw new Exception("Error searching songs by audio.", ex);
            }
        }

        public async Task<List<SongDto>> GetSongsBySingerIdAsync(int singerId)
        {
            if (singerId <= 0)
                throw new ArgumentException("Invalid singer id.", nameof(singerId));

            try
            {
                var songs = await _songRepository.GetSongsBySingerIdAsync(singerId);
                return _mapper.Map<List<SongDto>>(songs);
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving songs by singer id.", ex);
            }
        }
    }
}
