using Microsoft.Extensions.Options;
using TaktTusur.Media.Clients.VkApi;
using TaktTusur.Media.Clients.VkApi.Models;
using TaktTusur.Media.Core.Interfaces;
using TaktTusur.Media.Core.News;

namespace TaktTusur.Media.Infrastructure.VkSources;

public class ArticlesVkSource : IArticlesRemoteSource
{
	private readonly IEnvironment _environment;
	private readonly ArticlesVkSourceOptions _options;
	private readonly IVkApiClient _client;

	public ArticlesVkSource(IOptionsSnapshot<ArticlesVkSourceOptions> vkApiOptions, IEnvironment environment)
	{
		_environment = environment;
		_options = vkApiOptions.Value;
		_client = new VkApiClient(new VkApiOptions()
		{
			Key = _options.VkApiKey
		});
	}

	public bool IsPaginationSupported => false;
	
	public async Task<List<Article>> GetListAsync(CancellationToken token)
	{
		var data = await _client.GetPostsAsync(_options.GroupId, 20, token);
		var articles = data.Posts.Select(p => new Article()
		{
			Id = -1,
			Text = p.PostText,
			LastUpdated = _environment.GetCurrentDateTime(),
			OriginalSource = _options.GroupId,
			OriginalId = p.Id.ToString(),
			OriginalUpdatedAt = p.CreatedAt,
			OriginalCreatedAt = p.CreatedAt
		});
		return articles.ToList();
	}

	public async Task<(List<Article> entities, int totalCount)> GetListAsync(int skip, int take, CancellationToken token)
	{
		var data = await _client.GetPostsAsync(_options.GroupId, 10, token);
		var d = data;
		throw new NotImplementedException();
	}
}