using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Entity
{
    [Flags]
    public enum Types
    {
        שקט = 1,
        עוצמתי = 2,
        שמח = 4,
        מרגש = 8,
        ניגון = 16,
        אנרגטי = 32,
        חסידי = 64,
        מזרחי = 128,
        עממי = 256,
        נוסטלגי = 512,
        מקהלה = 1024,
        ווקאלי = 2048,
        שירת_ילדים = 4096,
        תפילה = 8192,
        התעוררות = 16384,
        רגש = 32768,
        הודאה = 65536,
        דבקות = 131072,
        פיוטי = 262144
    }

    [Flags]
    public enum Categories
    {
        שבת = 1,
        חגים = 2,
        פסח = 4,
        חנוכה = 8,
        פורים = 16,
        סוכות = 32,
        שבועות = 64,
        ראש_השנה = 128,
        יום_כיפור = 256,
        ימים_נוראים = 512,
        כללי = 1024,
        יום_העצמאות = 2048,
        חתונה = 4096,
        אירוסין = 8192,
        בר_מצווה = 16384,
        בת_מצווה = 32768,
        שמחות_משפחה = 65536,
        אבל = 131072,
        גאולה = 262144,
        משיח = 524288,
        אמונה = 1048576,
        תפילה = 2097152,
        לימוד_תורה = 4194304,
        ניגונים = 8388608,
        ילדים = 16777216,
        סיפורי_חזל = 33554432,
        חינוך = 67108864,
        חיזוק = 134217728,
        הודיה = 268435456,
        צדקה_וחסד = 536870912
    }
    public enum TargetAge
    {
        ילדים,
        מבוגרים
    }
    public class Song
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string Words { get; set; }
        public int NumViews { get; set; }
        public int SingerId { get; set; }
        [ForeignKey("SingerId")]
        public virtual Singer Singer { get; set; }
        public virtual ICollection<UserSongHistory> UserSongHistories { get; set; } = new List<UserSongHistory>();
        public virtual ICollection<User> UserLikes { get; set; } = new List<User>();
        public virtual ICollection<Feedback> Feedback { get; set; } = new List<Feedback>();
        public string VideoUrl { get; set; }
        public Types Type { get; set; }
        public Categories Category { get; set; }
        public TargetAge TargetAge { get; set; }
    }
    
}
