namespace TaktTusur.Media.Core.Interfaces;

public interface IReplicated
{
	public string? OriginalSource { get; }
	
	public string? OriginalId { get; }
	
	public DateTimeOffset? OriginalUpdatedAt { get; }
	
	public DateTimeOffset? OriginalCreatedAt { get; }
}