using Newtonsoft.Json;
using System.Globalization;

namespace Utils.Converters
{
    public class JsonDateTimeConverter : JsonConverter
    {

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            if (serializer == null)
                throw new ArgumentNullException(nameof(serializer));

            if (value == null)
            {
                writer.WriteNull();
            }
            else
            {
                writer.WriteValue(value);
            }
        }



        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            try
            {
                if (reader?.Value == null || string.IsNullOrEmpty(reader.Value.ToString()))
                {
                    return null;
                }

                if (objectType == typeof(DateTime))
                {
                    if (reader.Value is string dateString && DateTime.TryParse(dateString, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTime date))

                    {
                        return date.Kind == DateTimeKind.Local
                            ? Utils.Extensions.IOUtil.GetDateTime(date)
                            : date;
                    }

                    if (reader.Value is DateTime dateValue)
                    {
                        return dateValue.Kind == DateTimeKind.Local
                            ? Utils.Extensions.IOUtil.GetDateTime(dateValue)
                            : dateValue;
                    }
                }
                else if (objectType == typeof(DateTime?))
                {
                    if (reader.Value is string nullableDateString &&
                        DateTime.TryParse(nullableDateString, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTime nullableDate))

                    {
                        return nullableDate.Kind == DateTimeKind.Local
                            ? Utils.Extensions.IOUtil.GetDateTime(nullableDate)
                            : nullableDate;
                    }

                    if (reader.Value is DateTime nullableDateValue)
                    {
                        return nullableDateValue.Kind == DateTimeKind.Local
                            ? Utils.Extensions.IOUtil.GetDateTime(nullableDateValue)
                            : nullableDateValue;
                    }
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }


        public override bool CanConvert(Type objectType) => objectType == typeof(DateTime) || objectType == typeof(DateTime?);
    }
}