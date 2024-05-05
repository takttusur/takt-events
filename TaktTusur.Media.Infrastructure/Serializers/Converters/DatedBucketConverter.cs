using TaktTusur.Media.Core.DatedBucket;

namespace TaktTusur.Media.Infrastructure.Serializers.Converters;

using System.Text.Json;
using System.Text.Json.Serialization;

public class DatedBucketConverter<T> : JsonConverter<DatedBucket<T>>
{
	public override DatedBucket<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType != JsonTokenType.StartObject)
		{
			throw new JsonException();
		}

		var data = new List<T>();
		long id = 0; 
		while (reader.Read())
		{
			if (reader.TokenType == JsonTokenType.EndObject)
			{
				return DatedBucket<T>.CreateWith(id, data ?? []);
			}

			if (reader.TokenType == JsonTokenType.PropertyName)
			{
				string propertyName = reader.GetString();

				reader.Read(); 
                
				switch (propertyName)
				{
					case "Id":
						id = reader.GetInt64();
						break;
					case "Data":
						data = JsonSerializer.Deserialize<List<T>>(ref reader, options);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		throw new JsonException();
	}

	public override void Write(Utf8JsonWriter writer, DatedBucket<T> value, JsonSerializerOptions options)
	{
		writer.WriteStartObject();

		writer.WritePropertyName("Id");
		writer.WriteNumberValue(value.Id);

		writer.WritePropertyName("Data");
		JsonSerializer.Serialize(writer, value.Data, options);

		writer.WriteEndObject();
	}
}