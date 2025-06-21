using Microsoft.EntityFrameworkCore;
using Repository.Entity;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mock
{
    public class Database : DbContext, IContext
    {
        public DbSet<User> users { get ; set ; }
        public DbSet<Song> songs { get; set; }
        public DbSet<UserSongHistory> userSongHistories { get; set; }
        public DbSet<Singer> singers { get; set; }
        public DbSet<Feedback> feedbacks { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("server=DESKTOP-F6TEN9G;database=MusicProject;trusted_connection=true");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserSongHistory>()
                .HasKey(ush => new { ush.UserId, ush.SongId });
            base.OnModelCreating(modelBuilder);  
        }
        public async Task SaveAsync()
        {
            await SaveChangesAsync();
        }
    }
}


