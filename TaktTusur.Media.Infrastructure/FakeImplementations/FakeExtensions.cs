using TaktTusur.Media.Core.DatedBucket;
using TaktTusur.Media.Core.Interfaces;
using TaktTusur.Media.Core.News;

namespace TaktTusur.Media.Infrastructure.FakeImplementations;

public static class FakeExtensions
{
	public static IServiceCollection AddFakeRemoteSources(this IServiceCollection serviceCollection)
	{
		return serviceCollection.AddSingleton<IRemoteSource<Article>, FakeArticleRemoteSource>();
	}

	public static IServiceCollection AddFakeRepositories(this IServiceCollection serviceCollection)
	{
		return serviceCollection.AddSingleton<IRepository<DatedBucket<Article>>, FakeArticlesRepository>()
			.AddSingleton<IArticlesRepository, FakeArticlesRepository>();
	}
}