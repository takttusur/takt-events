using Quartz;
using TaktTusur.Media.Core.DatedBucket;
using TaktTusur.Media.Core.Entities;
using TaktTusur.Media.Core.Exceptions;
using TaktTusur.Media.Core.Interfaces;
using TaktTusur.Media.Core.Settings;

namespace TaktTusur.Media.Infrastructure.Jobs;

/// <summary>
/// Base job implementation for items replication from remote resources.
/// </summary>
/// <typeparam name="T">
/// Entity for replication,
/// should be <see cref="IIdentifiable"/> and <see cref="IReplicated"/>
/// </typeparam>
public abstract class ReplicationJobBase<T> : IJob where T: IIdentifiable, IReplicated
{
	protected const string StartWorkingMsg = $"{nameof(ReplicationJobBase<T>)} job is started";
	protected const string FinishWorkingMsg = $"{nameof(ReplicationJobBase<T>)} job is finished";
	protected const string InterruptedMsg = $"{nameof(ReplicationJobBase<T>)} job was interrupted";
	protected const string DisabledMsg = $"{nameof(ReplicationJobBase<T>)} job is disabled";
	
	private readonly IRemoteSource<T> _remoteSource;
	private readonly IRepository<DatedBucket<T>> _repository;
	private readonly ReplicationJobConfiguration _jobConfiguration;
	private readonly ILogger _logger;
	private readonly Queue<T> _brokenItems = new Queue<T>();
	
	/// <param name="remoteSource">Remote source of entity</param>
	/// <param name="repository">Repository for <see cref="T"/> </param>
	/// <param name="logger">Logger</param>
	/// <param name="jobConfiguration">Settings for the job, don't take it from DI</param>
	protected ReplicationJobBase(
		IRemoteSource<T> remoteSource, 
		IRepository<DatedBucket<T>> repository,
		ILogger logger,
		ReplicationJobConfiguration jobConfiguration)
	{
		_remoteSource = remoteSource;
		_repository = repository;
		_jobConfiguration = jobConfiguration;
		_logger = logger;
	}
	
	public async Task Execute(IJobExecutionContext context)
	{
		if (!_jobConfiguration.IsEnabled)
		{
			_logger.LogDebug(DisabledMsg);
			return;
		}

		try
		{
			_logger.LogDebug(StartWorkingMsg);
			if (_remoteSource.IsPaginationSupported)
			{
				await FetchByChunks();
			}
			else
			{
				await FetchByOneRequest();
			}

			if (_brokenItems.Count != 0)
			{
				ProcessBrokenItems(_brokenItems);
			}
		}
		catch (RemoteReadingException e)
		{
			_logger.LogWarning(e, InterruptedMsg);
		}
		catch (RepositoryReadingException e)
		{
			_logger.LogError(e, InterruptedMsg);
		}
		catch (RepositoryWritingException e)
		{
			_logger.LogError(e, InterruptedMsg);
		}
		finally
		{
			Cleanup();
			_logger.LogDebug(FinishWorkingMsg);
		}
	}
	
	/// <summary>
	/// The method should check is any similar item in <see cref="IRepository{TEntity}"/>.
	/// If we have the item, it should be updated and return true.
	/// If we don't have it - return false.
	/// </summary>
	/// <remarks>
	/// You don't have to call <see cref="IRepository{TEntity}.SaveAsync()"/> here.
	/// It will be called by <see cref="ReplicationJobBase{T}"/>
	/// </remarks>
	/// <param name="remoteItem">Item from remote resource.</param>
	/// <returns>true - if item was updated, false - if item not found.</returns>
	/// <example>
	/// <code>
	///	var item = _repository.GetByOriginalId(remoteItem.OriginalId);
	/// if (item == null) return false;
	/// item.Text = remoteItem.Text;
	/// _repository.Update(item);
	/// return true;
	/// </code>
	/// </example>
	protected abstract bool TryUpdateExistingItem(T remoteItem);

	/// <summary>
	/// Add new item to <see cref="IRepository{TEntity}"/>
	/// </summary>
	/// <param name="item">New item <see cref="IIdentifiable"/>, <see cref="IReplicated"/></param>
	/// <remarks>
	/// You don't have to call <see cref="IRepository{TEntity}.SaveAsync()"/> here.
	/// It will be called by <see cref="ReplicationJobBase{T}"/>
	/// </remarks>
	protected abstract void AddNewItem(T item);

	/// <summary>
	/// During the replicating process, some items can be broken.
	/// If you need to report about it - use brokenItemsQueue.
	/// </summary>
	/// <param name="brokenItemsQueue">Queue with broken items.</param>
	protected virtual void ProcessBrokenItems(Queue<T> brokenItemsQueue)
	{ }

	/// <summary>
	/// Cleanup after the job execution.
	/// </summary>
	protected virtual void Cleanup()
	{
		_brokenItems.Clear();
	}
	
	/// <summary>
	/// If <see cref="IRemoteSource{TEntity}"/> supports fetching by pages,
	/// items will be parsed by chunks.
	/// </summary>
	private async Task FetchByChunks()
	{
		var skip = 0;
		const int take = 10;
		int total;
		do
		{
			var (items, totalCount) = await _remoteSource.GetListAsync(skip, take);
			total = totalCount;
			
			await ProcessItems(items);

			skip += take;
		} while (skip < total);
	}

	/// <summary>
	/// If <see cref="IRemoteSource{TEntity}"/> doesn't support
	/// reading by chunks, and provides only one response - use this to read it.
	/// </summary>
	private async Task FetchByOneRequest()
	{
		var items = await _remoteSource.GetListAsync();
		if (items.Count > _jobConfiguration.MaxReplicatedItems)
		{
			items = items
				.OrderBy(a => a.OriginalUpdatedAt)
				.Take(_jobConfiguration.MaxReplicatedItems)
				.ToList();
		}
		await ProcessItems(items);
	}
	
	/// <summary>
	/// 
	/// </summary>
	/// <param name="items"></param>
	private async Task ProcessItems(List<T> items)
	{
		var counter = 0;
		foreach (var item in items)
		{
			if (string.IsNullOrEmpty(item.OriginalId))
			{
				_brokenItems.Enqueue(item);
				continue;
			}

			if (!TryUpdateExistingItem(item))
			{
				AddNewItem(item);	
			}

			counter++;
			
			if (counter < _jobConfiguration.CommitBuffer) continue;
			
			await _repository.SaveAsync();
			counter = 0;
		}

		if (counter != 0)
		{
			await _repository.SaveAsync();
		}
	}
}