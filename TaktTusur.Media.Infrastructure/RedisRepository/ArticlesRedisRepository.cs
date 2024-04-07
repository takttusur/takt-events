using StackExchange.Redis;
using TaktTusur.Media.Core.DatedBucket;
using TaktTusur.Media.Core.Interfaces;
using TaktTusur.Media.Core.News;
using TaktTusur.Media.Infrastructure.Serializers;

namespace TaktTusur.Media.Infrastructure.RedisRepository;

public class ArticlesRedisRepository(
	IConnectionMultiplexer redisConnection,
	IJsonSerializer<DatedBucket<Article>> jsonSerializer)
	: BaseRedisRepository<DatedBucket<Article>>(redisConnection, jsonSerializer, KeyPrefix), IArticlesRepository
{
	private const string KeyPrefix = $"media:article";
	public DatedBucket<Article>? FindBucketFor(DateTimeOffset createdAt)
	{
		throw new NotImplementedException();
	}
}