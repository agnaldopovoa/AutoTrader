using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Globalization;
using AutoTrader.Api.Constants;

namespace AutoTrader.Api.Converters;

public class CustomJsonConverterDateTimeNullable : JsonConverter<DateTime?>
{
    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        try
        {
            DateTime dataInformada = DateTime.ParseExact(
                reader.GetString()?[..Date.MaskLength] ?? string.Empty,
                Date.MaskFormat,
                CultureInfo.InvariantCulture);
                
            return TimeZoneInfo.ConvertTimeToUtc(
                dateTime: dataInformada,
                sourceTimeZone: TimeZoneInfo.FindSystemTimeZoneById(TimeZones.Brasilia));
        }
        catch (Exception)
        {
            throw new JsonException("Data inválida");
        }
    }

    public override void Write(Utf8JsonWriter writer, DateTime? date, JsonSerializerOptions options)
    {
        if (date.HasValue)
            writer.WriteStringValue(CustomJsonConverterDateTime.WriteValue(date.Value));
        else
            writer.WriteNullValue();
    }
}
