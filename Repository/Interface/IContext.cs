using Microsoft.EntityFrameworkCore;
using Repository.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface IContext
    {
        public DbSet<User> users { get; set; }
        public DbSet<Singer> singers { get; set; }
        public DbSet<Song> songs { get; set; }
        public DbSet<UserSongHistory> userSongHistories { get; set; }
        public DbSet<Feedback> feedbacks { get; set; }

        Task SaveAsync();
    }
}
