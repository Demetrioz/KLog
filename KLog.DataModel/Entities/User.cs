using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KLog.DataModel.Entities
{
    [Table("User")]
    public class User : KLogBase
    {
        [Key]
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public bool ResetRequired { get; set; }
        public DateTimeOffset LastLogin { get; set; }

        public ICollection<Application> Applications { get; set; }
    }
}
