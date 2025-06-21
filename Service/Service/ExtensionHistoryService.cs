using AutoMapper;
using Elastic.Clients.Elasticsearch;
using Repository.Entity;
using Repository.Interface;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Service
{
    public class ExtensionHistoryService : IExtensionHistoryService
    {
        private readonly IExtensionHistoryRepository _repository;
        private readonly IMapper _mapper;

        public ExtensionHistoryService(IExtensionHistoryRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<bool> GetAsync(int idUser, int idSong)
        {
            if (idUser <= 0 || idSong <= 0)
                throw new ArgumentException("Invalid id.", nameof(idUser));

            try
            {
                return await _repository.GetAsync(idUser, idSong);
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving user song history.", ex);
            }
        }

        public async Task<List<SongDto>> GetAllAsync(int idUser)
        {
            if (idUser <= 0)
                throw new ArgumentException("Invalid id.", nameof(idUser));

            try
            {
                var histories = await _repository.GetAllAsync(idUser);
                return _mapper.Map<List<SongDto>>(histories);

            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving user song history.", ex);
            }
        }


    }
}
