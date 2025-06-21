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
    public class FeedbackService : IService<FeedbackDto>
    {
        private readonly IRepository<Feedback> _repository;
        private readonly IMapper _mapper;
        public FeedbackService(IRepository<Feedback> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<FeedbackDto> AddAsync(FeedbackDto item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            try
            {
                var feedback = _mapper.Map<Feedback>(item);
                var addedFeedback = await _repository.AddAsync(feedback);
                return _mapper.Map<FeedbackDto>(addedFeedback);
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding feedback.", ex);
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
                throw new Exception("Error deleting feedback.", ex);
            }
        }

        public async Task<FeedbackDto> GetAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid id.", nameof(id));

            try
            {
                var feedback = await _repository.GetAsync(id);
                if (feedback == null)
                    throw new Exception("Feedback not found.");
                return _mapper.Map<FeedbackDto>(feedback);
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving feedback.", ex);
            }
        }

        public async Task<List<FeedbackDto>> GetAllAsync()
        {
            try
            {
                var feedbacks = await _repository.GetAllAsync();
                return _mapper.Map<List<FeedbackDto>>(feedbacks);
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving feedbacks.", ex);
            }
        }

        public async Task<FeedbackDto> UpdateAsync(int id, FeedbackDto item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (id <= 0)
                throw new ArgumentException("Invalid id.", nameof(id));

            try
            {
                var feedback = _mapper.Map<Feedback>(item);
                var updatedFeedback = await _repository.UpdateAsync(id, feedback);
                return _mapper.Map<FeedbackDto>(updatedFeedback);
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating feedback.", ex);
            }
        }
    }
}
