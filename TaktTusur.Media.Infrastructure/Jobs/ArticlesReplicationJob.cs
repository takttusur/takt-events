using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;
using TaktTusur.Media.Core.DatedBucket;
using TaktTusur.Media.Core.Interfaces;
using TaktTusur.Media.Core.News;
using TaktTusur.Media.Core.Settings;

namespace TaktTusur.Media.Infrastructure.Jobs;

public class ArticlesReplicationJob(
	IArticlesRemoteSource remoteSource,
	IArticlesRepository repository,
	ILogger<ArticlesReplicationJob> logger,
	IOptionsSnapshot<ReplicationJobConfiguration> jobSettings,
	IOptionsSnapshot<TextRestrictions> textRestrictions,
	ITextTransformer textTransformer,
	IEnvironment environment)
	: ReplicationJobBase<Article>(remoteSource, repository, logger, jobSettings.Get(nameof(ArticlesReplicationJob)))
{
	private const string BrokenArticleMessage = "The article {originalId} from {originalSource} is broken.";
	
	protected override bool TryUpdateExistingItem(Article remoteItem)
	{
		if (remoteItem.OriginalCreatedAt == null)
		{
			throw new ValidationException($"The object {remoteItem} doesn't have {nameof(Article.OriginalCreatedAt)} field");
		}
		var bucket = repository.FindBucketFor(remoteItem.OriginalCreatedAt.Value);
		var localArticle = bucket?.Data.FirstOrDefault(x =>
			x.OriginalId == remoteItem.OriginalId && x.OriginalSource == remoteItem.OriginalSource);
		
		if (bucket == null || localArticle == null) return false;
		if (remoteItem.OriginalUpdatedAt == localArticle.OriginalUpdatedAt) return true;
		
		var settings = textRestrictions.Get(nameof(ArticlesReplicationJob));
		
		localArticle.OriginalUpdatedAt = remoteItem.OriginalUpdatedAt;
		localArticle.Text =
			textTransformer.MakeShorter(remoteItem.Text, settings.MaxSymbolsCount, settings.MaxParagraphCount);
		localArticle.LastUpdated = environment.GetCurrentDateTime();
		bucket.SetChanged();
		repository.Update(bucket);
		return true;
	}

	protected override void AddNewItem(Article item)
	{
		if (item.OriginalCreatedAt == null)
		{
			// TODO:
			return;
		}
		var bucket = repository.FindBucketFor(item.OriginalCreatedAt.Value);
		var created = item.OriginalCreatedAt.Value;
		item.Text = textTransformer.MakeShorter(item.Text);
		item.LastUpdated = environment.GetCurrentDateTime();

		if (bucket != null)
		{
			bucket.Add(item);
			repository.Update(bucket);
		}
		else
		{
			bucket = DatedBucket<Article>.CreateWith(created, item);
			repository.Add(bucket);
		}
	}

	protected override void ProcessBrokenItems(Queue<Article> brokenItemsQueue)
	{
		while (brokenItemsQueue.TryDequeue(out var article))
		{
			logger.LogInformation(BrokenArticleMessage, article.OriginalId, article.OriginalSource);
		}
	}
}