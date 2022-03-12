using KLog.DataModel.Entities;
using Newtonsoft.Json;

namespace KLog.DataModel.Converters
{
    public class LogLevelConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }

        public override object? ReadJson(
            JsonReader reader,
            Type objectType,
            object? existingValue,
            JsonSerializer serializer
        )
        {
            try
            {
                string value = (string)reader.Value;

                switch (value)
                {
                    case "Info":
                        return LogLevel.Info;
                    case "Warning":
                        return LogLevel.Warning;
                    case "Error":
                        return LogLevel.Error;
                    case "Critical":
                        return LogLevel.Critical;
                    default:
                        return LogLevel.None;
                }
            }
            catch (Exception ex)
            {
                var test = Convert.ToInt32(reader.Value);
                return (LogLevel)test;
            }
        }

        public override void WriteJson(
            JsonWriter writer,
            object? value,
            JsonSerializer serializer
        )
        {
            LogLevel level = (LogLevel)value;

            switch (level)
            {
                case LogLevel.Info:
                    writer.WriteValue("Info");
                    break;
                case LogLevel.Warning:
                    writer.WriteValue("Warning");
                    break;
                case LogLevel.Error:
                    writer.WriteValue("Error");
                    break;
                case LogLevel.Critical:
                    writer.WriteValue("Critical");
                    break;
                case LogLevel.None:
                default:
                    writer.WriteNull(); 
                    break;
            }
        }
    }
}
