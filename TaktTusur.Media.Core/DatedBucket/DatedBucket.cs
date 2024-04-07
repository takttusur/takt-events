using TaktTusur.Media.Core.Interfaces;
using TaktTusur.Media.Core.Resources;

namespace TaktTusur.Media.Core.DatedBucket;

/// <summary>
/// Container to store objects by date.
/// </summary>
/// <typeparam name="T">Inner object type.</typeparam>
public class DatedBucket<T> : IIdentifiable
{
	private readonly List<T> _data;
	
	protected DatedBucket(int year, int month, int day, IList<T>? data = null)
	{
		DatedIdentifier = new DateTimeOffset(year, month, day, 0, 0, 0, TimeSpan.Zero);
		_data = data != null ? [..data] : [];
	}

	protected DatedBucket(long identifier, IList<T>? data = null)
	{
		var year = identifier / 100 / 100;
		var month = identifier / 100 % 100;
		var day = identifier % 100;
		if (year > int.MaxValue || month > int.MaxValue || day > int.MaxValue)  
			throw new ArgumentException(Localization.INVALID_IDENTIFIER_VALUE, nameof(identifier));

		DatedIdentifier = new DateTimeOffset((int)year, (int)month, (int)day, 0, 0, 0, TimeSpan.Zero);
		_data = data != null ? [..data] : [];
	}

	/// <summary>
	/// Represent <see cref="DatedIdentifier"/> as a number YYYYMMDD.
	/// </summary>
	/// <example>
	/// 2022-05-31 => 20220531.
	/// </example>
	public long Id => DateToIdentifier(DatedIdentifier.Year, DatedIdentifier.Month, DatedIdentifier.Day);
	
	/// <summary>
	/// Identifier of bucket.
	/// </summary>
	public DateTimeOffset DatedIdentifier { get; }

	public IReadOnlyList<T> Data => _data;

	public bool IsChanged { get; private set; } = false;

	public void Add(T item)
	{
		if (_data.Contains(item)) return;
		_data.Add(item);
		IsChanged = true;
	}

	public void Remove(T item)
	{
		if (!_data.Contains(item)) return;
		_data.Remove(item);
		IsChanged = true;
	}

	public void SetChanged()
	{
		IsChanged = true;
	}

	public static long DateToIdentifier(int year, int month, int day)
	{
		return ((year * 100) + month) * 100 + day;
	}

	public static DatedBucket<T> CreateEmpty(DateTimeOffset dateTime)
	{
		return new DatedBucket<T>(dateTime.Year, dateTime.Month, dateTime.Day);
	}

	public static DatedBucket<T> CreateWith(DateTimeOffset dateTime, T item)
	{
		var bucket = CreateEmpty(dateTime);
		bucket.Add(item);
		return bucket;
	}
}