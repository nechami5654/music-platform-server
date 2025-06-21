using AutoMapper;
using Repository.Interface;
using Repository.Repository;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Service
{
    public class ExtensionFeedbackService : IFeedbackService
    {
        private readonly IMapper _mapper;
        private readonly IFeedbackRepository _feedbackRepository;
        public ExtensionFeedbackService(IMapper mapper, IFeedbackRepository feedbackRepository)
        {
            _mapper = mapper;
            _feedbackRepository = feedbackRepository;
        }
        public async Task<List<object>> GetFeedbacksWithUsersBySongAsync(int idSong)
        {
            try
            {
                var feedbacks = await _feedbackRepository.GetFeedbacksWithUsersBySongAsync(idSong);
                return feedbacks;
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving feedbacks with user details.", ex);
            }
        }

    }
}
