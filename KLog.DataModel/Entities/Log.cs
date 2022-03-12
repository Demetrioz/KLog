using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KLog.DataModel.Entities
{
    public enum LogLevel 
    { 
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
        public DateTimeOffset Timestamp { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public LogLevel Level { get; set; }

        public string Source { get; set; }
        public string? Subject { get; set; }
        public string? Component { get; set; }
        public string Message { get; set; }
        public string? Data { get; set; }
    }
}
