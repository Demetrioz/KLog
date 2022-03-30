using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KLog.DataModel.Entities
{
    [Table("Application")]
    public class Application : KLogBase
    {
        [Key]
        public int ApplicationId { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }
        public string Key { get; set; }

        public ICollection<Log> Logs { get; set; }
    }
}
