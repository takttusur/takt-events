namespace TaktTusur.Media.Core.Interfaces;

/// <summary>
/// Remote readonly resource for entities
/// Provides paginated results and not-paginated
/// </summary>
/// <typeparam name="TEntity">Entity</typeparam>
public interface IRemoteSource<TEntity>
{
	/// <summary>
	/// If true - you can use GetList method to get paginated results.
	/// </summary>
	public bool IsPaginationSupported { get; }

	/// <summary>
	/// Get all data by one request, but the amount can be limited.
	/// </summary>
	/// <returns></returns>
	public Task<List<TEntity>> GetListAsync(CancellationToken token);

	/// <summary>
	/// Get paginated list of entities.
	/// </summary>
	/// <param name="skip">Skip several entities</param>
	/// <param name="take">Take several entities</param>
	/// <returns>Entities and amount of it</returns>
	public Task<(List<TEntity> entities, int totalCount)> GetListAsync(int skip, int take, CancellationToken token);
}