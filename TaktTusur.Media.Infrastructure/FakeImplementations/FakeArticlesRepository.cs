using TaktTusur.Media.Core.DatedBucket;
using TaktTusur.Media.Core.Interfaces;
using TaktTusur.Media.Core.News;

namespace TaktTusur.Media.Infrastructure.FakeImplementations;

public class FakeArticlesRepository : FakeRepository<DatedBucket<Article>> ,IArticlesRepository
{
	public DatedBucket<Article>? FindBucketFor(DateTimeOffset date)
	{
		var key = DatedBucket<Article>.DateToIdentifier(date.Year, date.Month, date.Day);
		return _db.GetValueOrDefault(key);
	}
}