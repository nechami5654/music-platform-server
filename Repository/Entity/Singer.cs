using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Entity
{
    [Index(nameof(Name), IsUnique = true)]
    public class Singer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string ImageUrl { get; set; }
        public virtual ICollection<Song> MySongs { get; set; } = new List<Song>();
    }
}
