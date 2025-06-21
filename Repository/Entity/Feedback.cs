using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Entity
{
    public class Feedback
    {
        public int Id { get; set; }
        public int IdSong { get; set; }
        [ForeignKey("IdSong")]
        public virtual Song Song { get; set; }
        public int IdUser { get; set; }
        [ForeignKey("IdUser")]
        public virtual User User { get; set; }
        public string Content { get; set; }
        public DateTime Date { get; set; }
        public bool IsNegative { get; set; }
    }
}
