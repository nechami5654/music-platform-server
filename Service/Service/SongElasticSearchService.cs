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
    public class SongElasticSearchService : ISongElasticSearchService
    {
        private readonly ISongElasticSearchRepository _repository;
        private readonly IMapper _mapper;

        public SongElasticSearchService(ISongElasticSearchRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<SongDto>> SearchSongsByLyricsAsync(string lyrics)
        {
            if (string.IsNullOrWhiteSpace(lyrics))
                throw new ArgumentException("Lyrics query is required.", nameof(lyrics));

            try
            {
                var songs = await _repository.SearchSongsByLyricsAsync(lyrics);
                return _mapper.Map<List<SongDto>>(songs);
            }
            catch (Exception ex)
            {
                throw new Exception("Error searching songs by lyrics.", ex);
            }
        }

        public async Task<List<SongDto>> FreeSearchAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Search text is required.", nameof(text));

            try
            {
                var songs = await _repository.FreeSearchAsync(text);
                return _mapper.Map<List<SongDto>>(songs);
            }
            catch (Exception ex)
            {
                throw new Exception("Error performing free search.", ex);
            }
        }
    }
}
