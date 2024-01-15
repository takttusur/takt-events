using TaktTusur.Media.Domain.Common;

namespace TaktTusur.Media.Domain.Events;

public class PublicEvent
{
    /// <summary>
    /// ���������� � ������ ���� � ����� ������ �������.
    /// </summary>
    public int EventStartDateTime { get; set; }

    /// <summary>
    /// ���������� � ������ ���� � ����� ���������� �������.
    /// </summary>
    public int EventEndDateTime { get; set; }

    /// <summary>
    /// ���������� � ������ �������� �������.
    /// </summary>
    public string EventTitle { get; set; }

    /// <summary>
    /// ���������� � ������ ������������.
    /// </summary>
    public List<Attachment> Attachments { get; set; }

    /// <summary>
    /// ���������� � ������ URL-������ �� ������ �������.
    /// </summary>
    public string EventURL { get; set; }
}