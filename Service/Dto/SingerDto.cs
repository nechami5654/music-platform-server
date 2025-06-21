using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Entity
{
    public class SingerDto
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public byte[]? Image { get; set; }
        public IFormFile File { get; set; }
        public virtual ICollection<SongDto>? MySongs { get; set; }
    }
}
