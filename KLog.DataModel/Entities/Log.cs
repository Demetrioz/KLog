using KLog.DataModel.Converters;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KLog.DataModel.Entities
{
    public enum LogLevel 
    { 
        None,
        Info,
        Warning,
        Error,
        Critical
    }

    [Table("Log")]
    public class Log : KLogBase
    {
        [Key]
        public int LogId { get; set; }

        [Required]
        public DateTimeOffset Timestamp { get; set; }

        [Required]
        [JsonConverter(typeof(LogLevelConverter))]
        public LogLevel Level { get; set; }

        public int ApplicationId { get; set; }
        public string? Source { get; set; }
        public string? Subject { get; set; }
        public string? Component { get; set; }

        [Required]
        public string Message { get; set; }

        public string? Data { get; set; }
    }
}
