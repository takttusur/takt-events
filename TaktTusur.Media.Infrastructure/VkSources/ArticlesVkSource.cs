using TaktTusur.Media.Core.Interfaces;
using TaktTusur.Media.Core.News;

namespace TaktTusur.Media.Infrastructure.VkSources;

public class ArticlesVkSource : IArticlesRemoteSource
{
	public bool IsPaginationSupported { get; }
	public Task<List<Article>> GetListAsync()
	{
		throw new NotImplementedException();
	}

	public Task<(List<Article> entities, int totalCount)> GetListAsync(int skip, int take)
	{
		throw new NotImplementedException();
	}
}