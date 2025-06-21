using Repository.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Dto
{
    public class FeedbackDto
    {
        public int? Id { get; set; }

        public int IdSong { get; set; }
        public int IdUser { get; set; }
        public string Content { get; set; }
        public DateTime? Date { get; set; }
        public bool IsNegative { get; set; }

    }
}
