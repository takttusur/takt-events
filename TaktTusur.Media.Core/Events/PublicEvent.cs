namespace TaktTusur.Media.Domain.Events;

using TaktTusur.Media.BackgroundCrawling.Core.Entities;

public class PublicEvent : IIdentifiable
{
    /// <summary>
    /// Идентификатор события.
    /// </summary>
    public long Id { get; set; }
    
    /// <summary>
    /// Дата и время начала события.
    /// </summary>
    public DateTimeOffset Start { get; set; }

    /// <summary>
    /// Дата и время окончания события.
    /// </summary>
    public DateTimeOffset? Finish { get; set; }

    /// <summary>
    /// Название события.
    /// </summary>
    public string Title { get; set; }
    
    /// <summary>
    /// Ссылка на оригинал события.
    /// </summary>
    public string? OriginalUrl { get; set; }
}