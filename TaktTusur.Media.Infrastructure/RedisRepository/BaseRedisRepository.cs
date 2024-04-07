using Microsoft.Extensions.Caching.StackExchangeRedis;
using StackExchange.Redis;
using TaktTusur.Media.Core.Interfaces;
using TaktTusur.Media.Infrastructure.Serializers;

namespace TaktTusur.Media.Infrastructure.RedisRepository;

/// <summary>
/// Base <see cref="Interfaces.IRepository{TEntity}"/> implementation using Redis cache.
/// Non thread safe.
/// Use transactions approach, don't forget to call Save finally.
/// </summary>
/// <typeparam name="T">DB Entity</typeparam>
public abstract class BaseRedisRepository<T> : IRepository<T> where T: class,IIdentifiable
{
	protected const string KeyDelimiter = ":";
	private readonly IJsonSerializer<T> _jsonSerializer;
	private readonly string _objectBaseKey;
	private readonly IDatabase _db;
	
	private ITransaction? _transaction = null;
	private List<Task> _unprocessedChanges = new List<Task>();

	/// <summary>
	/// </summary>
	/// <param name="redisConnection"></param>
	/// <param name="jsonSerializer"></param>
	/// <param name="objectBaseKey">
	/// The key base for stored object.
	/// For example, ClassName = MyEntity, Id of MyEntity instance = 5. The key can be baseKey+id => MyEntity5.
	/// </param>
	protected BaseRedisRepository(IConnectionMultiplexer redisConnection, IJsonSerializer<T> jsonSerializer, string objectBaseKey)
	{
		if (string.IsNullOrWhiteSpace(objectBaseKey))
		{
			throw new ArgumentException("Base key cannot be null, empty or whitespace", nameof(objectBaseKey));
		}

		_jsonSerializer = jsonSerializer;
		_objectBaseKey = objectBaseKey;
		_db = redisConnection.GetDatabase();
	}

	protected T? Find(long id)
	{
		var key = GetEntityKey(id);
		var value = _db.StringGet(key);
		if (value.IsNullOrEmpty) return null;

		return _jsonSerializer.Deserialize(value!);
	}
	
	public virtual void Add(T entity)
	{
		if (_transaction == null)
		{
			CreateTransaction();
		}

		var key = new RedisKey(GetEntityKey(entity));
		var value = new RedisValue(_jsonSerializer.Serialize(entity));
		
		var task = _transaction!.StringSetAsync(key, value);
		task.ConfigureAwait(false);
		_unprocessedChanges.Add(task);
	}

	public virtual void Delete(T entity)
	{
		Delete(entity.Id);
	}

	public void Update(T entity)
	{
		var newEntityKey = GetEntityKey(entity);

		if (_transaction == null)
		{
			CreateTransaction();
		}

		var redisKey = new RedisKey(newEntityKey);
		var value = new RedisValue(_jsonSerializer.Serialize(entity));

		var task = _transaction!.StringSetAsync(redisKey, value);
		task.ConfigureAwait(false);
		_unprocessedChanges.Add(task);
	}

	public virtual void Delete(long id)
	{
		if (_transaction == null)
		{
			CreateTransaction();
		}

		var key = new RedisKey(GetEntityKey(id));

		var task = _transaction!.StringGetDeleteAsync(key);
		task.ConfigureAwait(false);
		_unprocessedChanges.Add(task);
	}

	public bool Save()
	{
		if (_transaction == null) return false;
		var result = _transaction.Execute();
		Task.WaitAll(_unprocessedChanges.ToArray());
		return result;
	}

	public async Task<bool> SaveAsync()
	{
		if (_transaction == null) return false;
		var result = _transaction.ExecuteAsync();
		await Task.WhenAll(_unprocessedChanges);
		return await result;
	}

	protected string GetFullKey(string key)
	{
		return $"{_objectBaseKey}{KeyDelimiter}{key}";
	}
	
	protected string GetEntityKey(IIdentifiable identifiable)
	{
		return GetEntityKey(identifiable.Id);
	}
	
	protected string GetEntityKey(long id)
	{
		return GetFullKey(id.ToString());
	}
	
	private void CreateTransaction()
	{
		_transaction = _db.CreateTransaction();
		_unprocessedChanges = [];
	}
}