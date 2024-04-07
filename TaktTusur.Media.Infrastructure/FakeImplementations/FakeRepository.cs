using TaktTusur.Media.Core.Exceptions;
using TaktTusur.Media.Core.Interfaces;

namespace TaktTusur.Media.Infrastructure.FakeImplementations;

public class FakeRepository<T> : IRepository<T> where T : IIdentifiable
{
	protected Queue<Action> _currentTransaction = new Queue<Action>();

	protected Dictionary<long, T> _db = new Dictionary<long, T>();
	
	public virtual void Add(T entity)
	{
		_currentTransaction.Enqueue(() =>
		{
			var db = _db;
			db[entity.Id] = entity;
		});
	}

	public virtual void Delete(T entity)
	{
		_currentTransaction.Enqueue(() =>
		{
			var db = _db;
			if (!_db.ContainsKey(entity.Id))
			{
				throw new RepositoryWritingException();
			}

			db.Remove(entity.Id);
		});
	}

	public virtual void Update(T entity)
	{
		_currentTransaction.Enqueue(() =>
		{
			var db = _db;
			if (!_db.ContainsKey(entity.Id))
			{
				throw new RepositoryWritingException();
			}
			db[entity.Id] = entity;
		});
	}

	public virtual bool Save()
	{
		while (_currentTransaction.TryDequeue(out var action))
		{
			action.Invoke();
		}

		return true;
	}

	public virtual Task<bool> SaveAsync()
	{
		Save();
		return Task.FromResult(true);
	}
}