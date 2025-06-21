using Microsoft.AspNetCore.Http;
using Service.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Entity
{
  
    public class SongDto
    {
        public int? Id { get; set; }

        public string Name { get; set; }
        public DateTime? Date { get; set; }
        public string Words { get; set; }
        public int? NumViews { get; set; }
        public int SingerId { get; set; }
        public virtual ICollection<UserSongHistoryDto>? UserSongHistories { get; set; }
        public virtual ICollection<UserDto>? UserLikes { get; set; }
        public virtual ICollection<FeedbackDto>? Feedback{ get; set; }
        public byte[]? Video { get; set; }
        public IFormFile File { get; set; }
        public string Type { get; set; }
        public string Category { get; set; }
        public TargetAge TargetAge { get; set; }
    }
    
}
