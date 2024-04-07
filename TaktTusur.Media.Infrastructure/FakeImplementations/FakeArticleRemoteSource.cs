using TaktTusur.Media.Core.Interfaces;
using TaktTusur.Media.Core.News;

namespace TaktTusur.Media.Infrastructure.FakeImplementations;

/// <summary>
/// Generates fake articles, to don't use real remote resource.
/// </summary>
public class FakeArticleRemoteSource : IArticlesRemoteSource
{
	public const int TotalCount = 21;
	
	public bool IsPaginationSupported { get; } = true;
	public async Task<List<Article>> GetListAsync()
	{
		var result = await GetListAsync(0, 10);
		return result.entities;
	}

	public async Task<(List<Article> entities, int totalCount)> GetListAsync(int skip, int take)
	{
		var list = new List<Article>();
		for (int i = skip; i < Math.Min(skip + take, TotalCount); i++)
		{
			list.Add(GenerateArticle(i.ToString()));
		}
		return (list,TotalCount);
	}

	public static Article GenerateArticle(string originalId)
	{
		return new Article
		{
			Id = 0,
			Text = $"This is a fake article with originalId: {originalId} Update: {DateTime.Now:O}",
			LastUpdated = DateTimeOffset.UtcNow,
			OriginalSource = "FakeSource",
			OriginalId = originalId,
			OriginalUpdatedAt = DateTimeOffset.UtcNow,
			OriginalCreatedAt = DateTimeOffset.UtcNow
		};
	}
}