using TaktTusur.Media.Core.Interfaces;

namespace TaktTusur.Media.Core.News;

public class Article : IIdentifiable, IReplicated
{
    public long Id { get; set; }
    
    public string Text { get; set; }
    
    public DateTimeOffset LastUpdated { get; set; }
    
    public string? OriginalSource { get; set; }
    
    public string? OriginalId { get; set; }
    
    public DateTimeOffset? OriginalUpdatedAt { get; set; }
    
    public DateTimeOffset? OriginalCreatedAt { get; set; }
}