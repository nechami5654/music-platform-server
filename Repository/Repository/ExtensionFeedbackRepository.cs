using Microsoft.EntityFrameworkCore;
using Repository.Entity;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repository
{
    public class ExtensionFeedbackRepository : IFeedbackRepository
    {
        private readonly IContext _context;

        public ExtensionFeedbackRepository(IContext context)
        {
            _context = context;
        }
        public async Task<User> GetUserAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid user id.", nameof(id));

            try
            {
                return await _context.users.FirstOrDefaultAsync(x => x.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving user.", ex);
            }
        }
        public async Task<List<object>> GetFeedbacksWithUsersBySongAsync(int idSong)
        {
            try
            {
                if (idSong <= 0)
                    throw new ArgumentException("Invalid song id.", nameof(idSong));

                var feedbacks = await _context.feedbacks
                    .Where(f => f.IdSong == idSong) 
                    .Include(f => f.User) 
                    .Select(f => new
                    {
                        Id = f.Id,
                        IdSong = f.IdSong,
                        Content = f.Content,
                        Date = f.Date,
                        IsNegative = f.IsNegative,
                        UserName = f.User != null ? f.User.Name : "Unknown User", 
                        UserProfileImage = f.User != null ? f.User.ImageUrl : null 
                    })
                    .ToListAsync<object>();

                return feedbacks;
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving feedbacks with user details.", ex);
            }
        }



    }
}
