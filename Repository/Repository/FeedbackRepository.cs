using Microsoft.EntityFrameworkCore;
using Repository.Entity;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repository.Repository
{
    public class FeedbackRepository : IRepository<Feedback>
    {
        private readonly IContext _context;
        private readonly EmailService _emailService;

        public FeedbackRepository(IContext context, EmailService emailService) 
        {
            _context = context;
            _emailService = emailService; 
        }

        public async Task<Feedback> AddAsync(Feedback item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            try
            {
                if (item.IsNegative)
                {
                    int count = await _context.feedbacks.CountAsync(x => x.IsNegative && x.IdSong == item.IdSong);
                    if (count > 10)
                    {
                        
                        var idSinger = _context.songs.FirstOrDefault(x => x.Id == item.IdSong)?.SingerId;
                        var singer = _context.singers.FirstOrDefault(x => x.Id == idSinger);
                        var songName = _context.songs.FirstOrDefault(x => x.Id == item.IdSong)?.Name;
                        await _emailService.SendEmailAsync(
                            singer.Email,
                            singer.Name,
                            "Song Removed Due to Negative Feedback",
                            $"The song {songName} has been removed due to excessive negative feedback."
                        );

                        var list = await _context.feedbacks.Where(x => x.IdSong == item.IdSong).ToListAsync();
                        foreach (var f in list)
                        {
                            _context.feedbacks.Remove(f);
                        }
                        var song = await _context.songs.FirstOrDefaultAsync(x => x.Id == item.IdSong);
                        if (song != null)
                        {
                            _context.songs.Remove(song);
                        }
                        await _context.SaveAsync();
                        return item;
                    }
                }

                var existingFeedback = await _context.feedbacks.FirstOrDefaultAsync(x => x.IdUser == item.IdUser && x.IdSong == item.IdSong && x.IsNegative);
                if (existingFeedback != null && !item.IsNegative)
                {
                    await _context.feedbacks.AddAsync(item);
                    await _context.SaveAsync();
                }
                else if (existingFeedback == null)
                {
                    await _context.feedbacks.AddAsync(item);
                    await _context.SaveAsync();
                }
                return item;
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
                var feedback = await GetAsync(id);
                if (feedback == null)
                    throw new Exception("Feedback not found.");

                _context.feedbacks.Remove(feedback);
                await _context.SaveAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting feedback.", ex);
            }
        }

        public async Task<Feedback> GetAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid id.", nameof(id));

            try
            {
                return await _context.feedbacks.FirstOrDefaultAsync(x => x.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving feedback.", ex);
            }
        }

        public async Task<List<Feedback>> GetAllAsync()
        {
            try
            {
                return await _context.feedbacks.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving all feedbacks.", ex);
            }
        }

        public async Task<Feedback> UpdateAsync(int id, Feedback item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (id <= 0)
                throw new ArgumentException("Invalid id.", nameof(id));

            try
            {
                var feedback = await GetAsync(id);
                if (feedback == null)
                    throw new Exception("Feedback not found.");

                feedback.Content = item.Content;
                await _context.SaveAsync();
                return feedback;
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating feedback.", ex);
            }
        }
    }
}
