using System.Text.Json;
using TaktTusur.Media.Core.DatedBucket;
using TaktTusur.Media.Infrastructure.Serializers.Converters;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace TaktTusur.Media.Infrastructure.Serializers;

public class DatedBucketSerializer<T> : IJsonSerializer<DatedBucket<T>>
{
	private readonly JsonSerializerOptions _options = new()
	{
		Converters =
		{
			new DatedBucketConverter<T>()
		}
	};
	
	public string Serialize(DatedBucket<T> o)
	{
		return JsonSerializer.Serialize(o, _options);
	}

	public DatedBucket<T>? Deserialize(string json)
	{
		return JsonSerializer.Deserialize<DatedBucket<T>>(json, _options);
	}
}