using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Entity
{
    public class UserSongHistoryDto
    {
        public int? Id { get; set; }
        public int UserId { get; set; }
        public int SongId { get; set; }
        public DateTime? PlayedAt { get; set; }  
    }
}
