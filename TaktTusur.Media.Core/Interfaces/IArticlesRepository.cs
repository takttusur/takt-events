using TaktTusur.Media.Core.DatedBucket;
using TaktTusur.Media.Core.News;

namespace TaktTusur.Media.Core.Interfaces;

/// <summary>
/// The <see cref="IRepository{TEntity}"/> for <see cref="Article"/>
/// </summary>
public interface IArticlesRepository : IRepository<DatedBucket<Article>>
{
	public DatedBucket<Article>? FindBucketFor(DateTimeOffset createdAt);
}